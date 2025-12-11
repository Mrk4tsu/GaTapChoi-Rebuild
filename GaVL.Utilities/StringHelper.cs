using System.Text;
using System.Text.RegularExpressions;

namespace GaVL.Utilities
{
    public class StringHelper
    {
        public static string GenerateSeoAlias(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            input = input.Trim();
            input = input.Replace("–", "-").Replace("—", "-");

            input = Regex.Replace(input, @"[^\w\s-]", "");
            input = input.Replace(".", "-")
                         .Replace(",", "-")
                         .Replace(";", "-")
                         .Replace(":", "-");
            input = Regex.Replace(input, @"\s+", "-");
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string str = input.Normalize(NormalizationForm.FormD);
            string str2 = regex.Replace(str, string.Empty).Replace('đ', 'd').Replace('Đ', 'D');
            str2 = Regex.Replace(str2, "-{2,}", "-");
            return str2.Trim('-').ToLower();
        }
    }
}
