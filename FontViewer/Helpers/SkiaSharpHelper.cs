using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontViewer.Helpers
{
    internal static class SkiaSharpHelper
    {
        internal static string ReadFontNameTable(string filename)
        {
            using var typeface = SkiaSharp.SKTypeface.FromFile(filename);
            var nameTableData = typeface.GetTableData(0x6E616D65); // 'name' table tag in hex
            var nameTableInfo = ParseNameTable(nameTableData);
            return nameTableInfo;
        }

        private static string ParseNameTable(byte[] nameTableData)
        {
            var nameTableInfo = new StringBuilder();
            int format = BitConverter.ToUInt16(nameTableData.Take(2).Reverse().ToArray());
            int count = BitConverter.ToUInt16(nameTableData.Skip(2).Take(2).Reverse().ToArray());
            int stringOffset = BitConverter.ToUInt16(nameTableData.Skip(4).Take(2).Reverse().ToArray());

            for (int i = 0; i < count; i++)
            {
                int recordOffset = 6 + i * 12;
                int platformID = BitConverter.ToUInt16(nameTableData.Skip(recordOffset).Take(2).Reverse().ToArray());
                int encodingID = BitConverter.ToUInt16(nameTableData.Skip(recordOffset + 2).Take(2).Reverse().ToArray());
                int languageID = BitConverter.ToUInt16(nameTableData.Skip(recordOffset + 4).Take(2).Reverse().ToArray());
                int nameID = BitConverter.ToUInt16(nameTableData.Skip(recordOffset + 6).Take(2).Reverse().ToArray());
                int length = BitConverter.ToUInt16(nameTableData.Skip(recordOffset + 8).Take(2).Reverse().ToArray());
                int offset = BitConverter.ToUInt16(nameTableData.Skip(recordOffset + 10).Take(2).Reverse().ToArray());

                var nameBytes = new byte[length];
                Array.Copy(nameTableData, stringOffset + offset, nameBytes, 0, length);

                // 根据平台ID和编码ID选择合适的编码
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Encoding encoding = platformID switch
                {
                    0 => Encoding.BigEndianUnicode, // Unicode
                    1 => languageID switch
                    {
                        0 => Encoding.Latin1, // Latin1
                        33 => Encoding.GetEncoding(10008), // Chinese
                        _ => Encoding.GetEncoding(10008), // Fallback to Chinese for other languages
                    }, // Macintosh
                    3 => Encoding.BigEndianUnicode, // Windows
                    _ => Encoding.UTF8, // Fallback to UTF-8
                };

                var nameString = encoding.GetString(nameBytes);

                nameTableInfo.AppendLine($"NameID: {nameID}, PlatformID: {platformID}, EncodingID: {encodingID}, LanguageID: {languageID}, Name: {nameString}");
            }

            return nameTableInfo.ToString();
        }

        internal static void PrintGlyphOutlinePoints(string filename, char character)
        {
        }
    }
}
