using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicodeQueryTool.Helpers
{
    internal static class UnicodeTextConverter
    {
        internal static string TextToUnicode(string text)
        {
            var unicodeString = new StringBuilder();
            foreach (char c in text)
            {
                unicodeString.AppendFormat("{0:X4} ", (int)c);
            }
            return unicodeString.ToString().Trim();
        }

        internal static string UnicodeToText(string unicode)
        {
            var text = new StringBuilder();
            var unicodeValues = unicode.Split(new[] { "\\u", " ", "0x", "0X", "uni", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var value in unicodeValues)
            {
                if (int.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int codePoint))
                {
                    text.Append((char)codePoint);
                }
            }
            return text.ToString();
        }
    }
}
