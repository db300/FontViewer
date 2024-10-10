using System;
using System.Collections.Generic;
using System.Text;

namespace FontViewer.Models
{
    public class FontItem
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        public WaterTrans.GlyphLoader.Typeface Typeface { get; set; }
    }
}
