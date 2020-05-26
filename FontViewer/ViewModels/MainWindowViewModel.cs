using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;

namespace FontViewer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region constructor

        public MainWindowViewModel()
        {
        }

        #endregion

        #region property



        #endregion

        #region command

        public ReactiveCommand<Unit, Unit> OpenCommand => ReactiveCommand.CreateFromTask(async () =>
        {
            var openDlg = new OpenFileDialog
            {
                AllowMultiple = true,
                Filters =
                {
                    new FileDialogFilter {Name = "字体文件(*.otf;*.ttf)", Extensions = new List<string> {"otf", "ttf"}},
                    new FileDialogFilter {Name = "OpenType文件(*.otf)", Extensions = new List<string> {"otf"}},
                    new FileDialogFilter {Name = "TrueType文件(*.ttf)", Extensions = new List<string> {"ttf"}},
                    new FileDialogFilter {Name = "所有文件(*.*)", Extensions = new List<string> {"*"}}
                }
            };
            var result = await openDlg.ShowAsync((Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow);
            if (!(result?.Length > 0)) return;
            System.Diagnostics.Debug.WriteLine(result);
            foreach (var fileName in result)
            {
                await using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                var typeface = new WaterTrans.GlyphLoader.Typeface(fs);
                System.Diagnostics.Debug.WriteLine(typeface);
            }
        });

        #endregion
    }
}
