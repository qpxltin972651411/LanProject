using LanProject.MainApplication.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace LanProject.MainApplication.Method
{
    public static class Function
    {
        private static readonly string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);//LanProject.Method.Configuration.ReadSetting("dataPath")
        private static readonly string foldername = LanProject.Method.Configuration.ReadSetting("appfoldername");
        private static readonly string notefolder = LanProject.Method.Configuration.ReadSetting("notes");
        /// <summary>
        /// 給予一個整數，若大於等於10則拆開位數彼此相加，返回加總值
        /// </summary>
        /// <param name="num">int</param>
        /// <returns>int</returns>
        public static int Sum(int num)
        {
            var total = num;
            if (total > 9)
            {
                var s = total.ToString();
                var n1 = (int.Parse(s.Substring(0, 1))) * 1;
                var n2 = (int.Parse(s.Substring(1, 1))) * 1;
                total = n1 + n2;
            }
            return total;
        }


        /// <summary>
        /// 給予一字串，驗證是否符合統編規則，符合則True，反之False
        /// </summary>
        /// <param name="InputTax">string</param>
        /// <returns>boolean</returns>
        public static bool TaxValid(string InputTax)
        {
            int total = 0;
            int[] cx = { 1, 2, 1, 2, 1, 2, 4, 1 };
            for (var i = 0; i < InputTax.Length; i++)
                total = total + Sum(((int)Char.GetNumericValue(InputTax[i])) * cx[i]);

            if ((total % 10) == 0) return true;

            if (InputTax[6] == '7' && ((total + 1) % 10 == 0)) return true;

            return false;
        }



        /// <summary>
        /// 給予一字元，驗證是否為中文，符合則True，反之False
        /// </summary>
        /// <param name="c">char</param>
        /// <returns>boolean</returns>
        public static bool IsChinese(char c)
        {
            Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
            return cjkCharRegex.IsMatch(c.ToString());
        }

        /// <summary>
        /// 給予一字串，驗證是否由數字組成，符合則True，反之False
        /// </summary>
        /// <param name="s">string</param>
        /// <returns>boolean</returns>
        public static bool IsNumber(string s)
        {
            for (var i = 0; i < s.Length; i++)
            {
                if (s[i] >= '0' && s[i] <= '9') continue;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 取得表單ID，{4位數年份}{2位數月份}{2位數日期}{5位數亂數}
        /// </summary>
        /// <returns>string</returns>
        public static string GetFormID() => string.Format("{0:0000}{1:00}{2:00}{3:00000}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, (new Random()).Next(100000));


        /// <summary>
        /// 檢查文件名稱是否有效，檢查是否為空，檢查是否以.結尾，檢查是否超過245字元，檢查是否包含非法字元，若有效則返回null
        /// </summary>
        /// <param name="filename">string</param>
        /// <returns>string</returns>
        public static string IsFilenameValid(string filename)
        {
            if (filename == String.Empty)
                return "名稱為空";
            if (filename.EndsWith("."))
                return "不得以.作為最後一個字元";
            if (filename.Length > 245)
                return "長度過長";
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                int matchs = filename.IndexOf(c);
                if (matchs == -1) continue;
                return String.Format("不得以非法字元 {0} 命名", c);
            }
            return null;
        }
        /// <summary>
        /// 帶入一參數，根據參數縣市名稱，返回該縣市所有行政區列表
        /// </summary>
        /// <param name="source">string</param>
        /// <returns>List<string></returns>
        public static List<string> RefreshCity(string source)
        {
            List<string> city = new List<string> { "無" };
            if (!RoadFileExisted())
                return city;
            if (source == "無")
                return city;
            string appFolderPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string filepath = Path.Combine(appFolderPath, "Resources", "opendata111road.csv");
            try
            {
                using (var reader = new StreamReader(filepath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        if (values[0] == source && !city.Contains(values[1].Replace(values[0], String.Empty)))
                            city.Add(values[1].Replace(values[0], String.Empty));
                    }
                }
                return city;
            }
            catch
            {
                return RefreshCity(source);
            }
        }
        /// <summary>
        /// 檢查台灣縣市行政區檔案是否存在且有效，若有效則返回True，反之False
        /// </summary>
        /// <returns>boolean</returns>
        public static bool RoadFileExisted()
        {
            string appFolderPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string filepath = Path.Combine(appFolderPath, "Resources", "opendata111road.csv");
            if (!File.Exists(filepath)) return false;
            using (var reader = new StreamReader(filepath))
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                if (values[0] == "city" && values[1] == "site_id" && values[2] == "road")
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 返回台灣縣市列表
        /// </summary>
        /// <returns>List<string></returns>
        public static List<string> CountryList()
        {
            List<string> country = new List<string> { "無" };
            if (!RoadFileExisted())
                return country;
            string appFolderPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string filepath = Path.Combine(appFolderPath, "Resources", "opendata111road.csv");
            try
            {
                int count = 0;
                using (var reader = new StreamReader(filepath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        count += 1;
                        if (count > 2)
                            if (!country.Contains(values[0])) country.Add(values[0]);
                    }
                }
                return country;
            }
            catch
            {
                return CountryList();
            }
        }
        /// <summary>
        /// 檢查輸入之CreateUnit，是否完全符合規則，是則返回True，反之False
        /// </summary>
        /// <param name="source">CreateUnit</param>
        /// <returns>boolean</returns>
        public static bool VerifyInput(CreateUnit source)
        {
            if (String.IsNullOrWhiteSpace(source.Name))
                return false;

            if (String.IsNullOrWhiteSpace(source.Cel))
                return false;
            if (source.Cel.Any(x => IsChinese(x)))
                return false;
            if (!IsNumber(source.Cel))
                return false;
            if (source.Cel.Length != 10)
                return false;

            if (source.HaveTax)
            {
                if (String.IsNullOrWhiteSpace(source.Tax))
                    return false;
                if (source.Tax.Any(x => IsChinese(x)))
                    return false;
                if (!IsNumber(source.Tax))
                    return false;
                if (source.Tax.Length != 8)
                    return false;
                if (!TaxValid(source.Tax))
                    return false;
            }

            if (source.HaveTel)
            {
                if (String.IsNullOrWhiteSpace(source.Tel.AreaCode))
                    return false;
                if (String.IsNullOrWhiteSpace(source.Tel.Number))
                    return false;

                if (source.Tel.AreaCode.Any(x => IsChinese(x)))
                    return false;
                if (source.Tel.Number.Any(x => IsChinese(x)))
                    return false;

                if (!IsNumber(source.Tel.AreaCode))
                    return false;
                if (!IsNumber(source.Tel.Number))
                    return false;

                if ((source.Tel.AreaCode.Length == 0) || (source.Tel.AreaCode.Length > 4))
                    return false;
                if ((source.Tel.Number.Length == 0) || (source.Tel.Number.Length > 8))
                    return false;
            }
            if (source.HaveFax)
            {
                if (String.IsNullOrWhiteSpace(source.Fax.AreaCode))
                    return false;
                if (String.IsNullOrWhiteSpace(source.Fax.Number))
                    return false;

                if (source.Fax.AreaCode.Any(x => IsChinese(x)))
                    return false;
                if (source.Fax.Number.Any(x => IsChinese(x)))
                    return false;

                if (!IsNumber(source.Fax.AreaCode))
                    return false;
                if (!IsNumber(source.Fax.Number))
                    return false;

                if ((source.Fax.AreaCode.Length == 0) || (source.Fax.AreaCode.Length > 4))
                    return false;
                if ((source.Fax.Number.Length == 0) || (source.Fax.Number.Length > 8))
                    return false;
            }


            if (source.HaveAddress)
            {
                if (String.IsNullOrWhiteSpace(source.Location.Country) || source.Location.Country.Equals("無"))
                    return false;
                if (String.IsNullOrWhiteSpace(source.Location.City) || source.Location.City.Equals("無"))
                    return false;
                if (String.IsNullOrWhiteSpace(source.Location.Address))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 返回輸入之UserControl之父Container，若無則返回null
        /// </summary>
        /// <param name="source">UserControl</param>
        /// <returns>MainWindow</returns>
        /// <returns>null</returns>
        public static MainWindow GetParent(UserControl source)
        {
            MainWindow main = null;
            var obj = LogicalTreeHelper.GetParent(source);
            while (obj.GetType() != typeof(MainWindow))
                obj = LogicalTreeHelper.GetParent(obj);
            main = obj as MainWindow;
            return main;
        }
        /// <summary>
        /// 返回相對應的Description
        /// </summary>
        /// <param name="value">Enum</param>
        /// <returns>string</returns>
        /// <returns>null</returns>
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                        return attr.Description;
                }
            }
            return null;
        }
        /// <summary>
        /// 給予一Unit List、Unit，檢查是否有重複，若有則返回True，否則False
        /// </summary>
        /// <param name="source">BindingList<Unit></param>
        /// <param name="target">CreateUnit</param>
        /// <returns>boolean</returns>
        public static bool UnitHaveRepeat(BindingList<Unit> source, CreateUnit target)
        {
            if (source.Any(vpk => (vpk.Name == target.Name) && (vpk.Cel == target.Cel) && (vpk.Tax == target.Tax)))
                return true;
            return false;
        }

        /// <summary>
        /// 給予一Unit List、Unit、原Unit，檢查是否有重複，若有則返回True，否則False
        /// </summary>
        /// <param name="source">BindingList<Unit></param>
        /// <param name="target">CreateUnit</param>
        /// <param name="nornal">CreateUnit</param>
        /// <returns>boolean</returns>
        public static bool EditUnitHaveRepeat(BindingList<Unit> source, CreateUnit target, CreateUnit nornal)
        {
            if (source.Count(vpk => (vpk.Name == target.Name) && (vpk.Cel == target.Cel) && (vpk.Tax == target.Tax)) == 1)
            {
                if (!(nornal.Name == target.Name && nornal.Cel == target.Cel && nornal.Tax == target.Tax))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 返回備註列表
        /// </summary>
        /// <returns>BindingList<string></returns>
        public static BindingList<string> ReadingNoteTitleList()
        {
            BindingList<string> NoteList = new BindingList<string> { "無" };
            Directory.CreateDirectory(Path.Combine(programFilesPath, foldername, notefolder));
            var fileLists = Directory.GetFiles(Path.Combine(programFilesPath, foldername, notefolder));
            foreach (var item in fileLists)
                NoteList.Add(Path.GetFileNameWithoutExtension(item));
            return NoteList;
        }
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
