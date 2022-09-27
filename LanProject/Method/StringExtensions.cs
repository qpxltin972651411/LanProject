using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace LanProject.Method
{
    public static class StringExtensions
    {
        /// <summary>
        /// 從字串中移除指定char，返回移除指定char之字串
        /// </summary>
        /// <param name="separators">multiple chars</param>
        /// <param name="newVal">New value</param>
        /// <returns>string</returns>
        public static string Replace(this string s, char[] separators, string newVal)
        {
            string[] temp;
            temp = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            return String.Join(newVal, temp);
        }
        public static bool IsChinese(char c)
        {
            Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
            return cjkCharRegex.IsMatch(c.ToString());
        }
        public static string OnlyAcceptLetterAndNumber(TextBox target)
        {
            if (String.IsNullOrEmpty(target.Text) || String.IsNullOrWhiteSpace(target.Text))
                return " * 帳號欄位不得為空";

            if (target.Text.Any(x => StringExtensions.IsChinese(x)))
                return " * 帳號僅能英數格式";

            for (var i = 0; i < target.Text.Length; i++)
            {
                if ((target.Text[i] >= '0' && target.Text[i] <= '9') || (target.Text[i] >= 'a' && target.Text[i] <= 'z') || (target.Text[i] >= 'A' && target.Text[i] <= 'Z'))
                    continue;
                return " * 帳號僅能英數格式";
            }
            return String.Empty;
        }
    }
    public static class Generate
    {
        /// <summary>
        /// 產生指定長度之隨機字串
        /// </summary>
        /// <param name="length">字串長度</param>
        /// <returns>string</returns>
        public static string RandomString(int length)
        {
            const string pool = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random rand = new Random();
            var builder = new StringBuilder();
            for (var i = 0; i < length; i++)
                builder.Append(pool[rand.Next(0, pool.Length)]);
            return builder.ToString();
        }

        /// <summary>
        /// 加密字串
        /// </summary>
        /// <param name="pToEncrypt">字串</param>
        /// <param name="sKey">加密鑰匙</param>
        /// <returns>string</returns>
        public static string MD5Encrypt(string pToEncrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
                ret.AppendFormat("{0:X2}", b);
            return ret.ToString();
        }
        /// <summary>
        /// 加密字串
        /// </summary>
        /// <param name="plainText">字串</param>
        /// <returns>string</returns>
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        /// <summary>
        /// 解密字串
        /// </summary>
        /// <param name="pToEncrypt">字串</param>
        /// <param name="sKey">解密鑰匙</param>
        /// <returns>string / null</returns>
        public static string MD5Decrypt(string pToDecrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            try
            {
                byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
                for (int x = 0; x < pToDecrypt.Length / 2; x++)
                    inputByteArray[x] = (byte)(Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();
                return Encoding.Default.GetString(ms.ToArray());
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 解密字串
        /// </summary>
        /// <param name="base64EncodedData">字串</param>
        /// <returns>string / null</returns>
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
    public static class SecureConverter
    {
        public static string SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
    }
}
