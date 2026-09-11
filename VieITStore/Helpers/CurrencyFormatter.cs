using System.Globalization;

namespace VieITStore.Helpers
{
    public static class CurrencyFormatter
    {
        /// <summary>
        /// Formats a decimal value to Vietnamese currency format (e.g., 1.900.000đ)
        /// </summary>
        public static string FormatVND(decimal value)
        {
            return value.ToString("#,##0", new CultureInfo("vi-VN")) + "đ";
        }

        /// <summary>
        /// Formats a decimal value to Vietnamese currency without suffix
        /// </summary>
        public static string FormatVNDNumber(decimal value)
        {
            return value.ToString("#,##0", new CultureInfo("vi-VN"));
        }

        /// <summary>
        /// Parses Vietnamese currency string to decimal (e.g., "1.900.000đ" => 1900000)
        /// </summary>
        public static decimal ParseVND(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            // Remove the đ suffix and spaces
            value = value.Replace("đ", "").Replace(" ", "").Trim();

            // Vietnamese currency uses . as thousand separator, not decimal separator
            // So we just need to remove the dots
            value = value.Replace(".", "");

            if (decimal.TryParse(value, out var result))
                return result;

            return 0;
        }
    }
}
