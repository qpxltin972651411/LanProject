using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace LanProject.MainApplication.Domain
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString())
                ? new ValidationResult(false, "* 欄位不得空白")
                : ValidationResult.ValidResult;
        }
    }
    public class TaxValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string inputtax = (string)value;

            if (string.IsNullOrWhiteSpace((inputtax ?? "").ToString()))
                return new ValidationResult(false, "* 該欄位不得空白");

            if (inputtax.Any(z => Method.Function.IsChinese(z)))
                return new ValidationResult(false, "* 該欄位僅能填寫數字");

            if (!Method.Function.IsNumber(inputtax))
                return new ValidationResult(false, "* 該欄位僅能填寫數字");

            if (inputtax.Length != 8)
                return new ValidationResult(false, "* 該欄位長度必須8位");

            if (!Method.Function.TaxValid(inputtax))
                return new ValidationResult(false, "* 統編檢查有誤");

            return ValidationResult.ValidResult;
        }
    }
    public class CelValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string inputcel = (string)value;

            if (string.IsNullOrWhiteSpace((inputcel ?? "").ToString()))
                return new ValidationResult(false, "* 該欄位不得空白");

            if (inputcel.Any(z => Method.Function.IsChinese(z)))
                return new ValidationResult(false, "* 該欄位僅能填寫數字");

            if (!Method.Function.IsNumber(inputcel))
                return new ValidationResult(false, "* 該欄位僅能填寫數字");

            if (inputcel.Length != 10)
                return new ValidationResult(false, "* 該欄位長度必須10位");

            return ValidationResult.ValidResult;
        }
    }
    public class AreaCodeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string inputareacode = (string)value;

            if (string.IsNullOrWhiteSpace((inputareacode ?? "").ToString()))
                return new ValidationResult(false, "* 欄位不得空白");

            if (inputareacode.Any(z => Method.Function.IsChinese(z)))
                return new ValidationResult(false, "* 僅能填寫數字");

            if (!Method.Function.IsNumber(inputareacode))
                return new ValidationResult(false, "* 僅能填寫數字");

            if (inputareacode.Length > 4)
                return new ValidationResult(false, "* 欄位長度上限4位");

            return ValidationResult.ValidResult;
        }
    }
    public class NumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string inputnumber = (string)value;

            if (string.IsNullOrWhiteSpace((inputnumber ?? "").ToString()))
                return new ValidationResult(false, "* 欄位不得空白");

            if (inputnumber.Any(z => Method.Function.IsChinese(z)))
                return new ValidationResult(false, "* 該欄位僅能填寫數字");

            if (!Method.Function.IsNumber(inputnumber))
                return new ValidationResult(false, "* 該欄位僅能填寫數字");

            if (inputnumber.Length > 8)
                return new ValidationResult(false, "* 欄位長度上限8位");

            return ValidationResult.ValidResult;
        }
    }
    public class AddressComboPickValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string address = (string)value;
            if (address == "無" || string.IsNullOrWhiteSpace((address ?? "").ToString()))
                return new ValidationResult(false, "* 該欄位必須填選");
            return ValidationResult.ValidResult;
        }
    }
    public class MoneyNotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string money = (string)value;
            double result = 0.0;
            bool IsValid = double.TryParse(money, out result);
            if (!IsValid)
                return new ValidationResult(false, "* 欄位格式有誤");
            if (Math.Abs(result) < 0.001)
                return new ValidationResult(false, "* 欄位不得為0");
            return ValidationResult.ValidResult;
        }
    }
    public class MoneyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string money = (string)value;
            double result = 0.0;
            bool IsValid = double.TryParse(money, out result);
            if (!IsValid)
                return new ValidationResult(false, "* 欄位格式有誤");
            return ValidationResult.ValidResult;
        }
    }
}
