using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontViewer.Helpers
{
    internal static class CommonHelper
    {
        internal static async Task<IReadOnlyList<IStorageFile>?> OpenFontFileAsync(Visual? visual)
        {
            var topLevel = TopLevel.GetTopLevel(visual);
            if (topLevel is null) return null;
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = true,
                FileTypeFilter = new List<FilePickerFileType>
                {
                    new("字库文件") { Patterns = new List<string> { "*.ttf", "*.otf" } },
                    new("ttf文件") { Patterns = new List<string> { "*.ttf" } },
                    new("otf文件") { Patterns = new List<string> { "*.otf" } },
                    new("所有文件") { Patterns = new List<string> { "*.*" } }
                }
            });
            return files;
        }
    }
}
