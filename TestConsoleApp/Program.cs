namespace TestConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            const string fileName = @"C:\Users\冷怀晶\Downloads\汉仪跑男体\HYKeepRunning-95U.ttf";
            const string fileName2 = @"C:\Users\冷怀晶\Downloads\汉仪跑男体\补字\HYKeepRunning-95U-经.ttf";
            //var helper=new iHawkSkiaSharpCommonLibrary.Helpers.FontHelper(fileName);
            var result = iHawkSkiaSharpCommonLibrary.Helpers.FontCompareHelper.CompareFonts(fileName, fileName2);
            //var helper = new iHawkSkiaSharpCommonLibrary.Helpers.SkiaSharpHelper(fileName);
            //helper.GetTypeSetting();
            /*
            var unicodeList = helper.GetAllUnicodeHex();
            var map = helper.GetUnicodeGidMap();
            var mapstring = string.Join("\r\n", map.Select(kv => $"{kv.Key:X4}={kv.Value}"));
            Console.WriteLine($"Total Unicode Count: {unicodeList?.Count}");
            */
        }
    }
}
