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
        public static string GenerateRandomCode(int length = 6)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder result = new StringBuilder(length);
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }
            return result.ToString();
        }
        public static string NormalizeTagName(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var str = input.Trim().ToLowerInvariant();
            str = Regex.Replace(str, @"\s+", " ");
            return str;
        }
        private string TruncateString(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;
            return text.Substring(0, maxLength) + "...";
        }
        public static string ExtractUsernameFromEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return string.Empty;

            var parts = email.Split('@');
            if (parts.Length == 0)
                return string.Empty;

            var localPart = parts[0];
            return RemoveSpecialCharacters(localPart);
        }
        public static string RemoveSpecialCharacters(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            return Regex.Replace(input, @"[^a-zA-Z0-9]", "");
        }
    }
}
