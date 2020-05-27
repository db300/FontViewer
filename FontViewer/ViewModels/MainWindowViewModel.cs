using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using FontViewer.Models;
using ReactiveUI;

namespace FontViewer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region constructor

        public MainWindowViewModel()
        {
            Items = _root.Children;
            FontName = "test";
        }

        #endregion

        #region property

        public ObservableCollection<FontItem> Items { get; set; }

        public FontItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                var typeface = _selectedItem?.Typeface;
                FontName = typeface == null ? "" : string.Join("\r\n", typeface.NAMEFeatures.Select(item => item.Value));
                System.Diagnostics.Debug.WriteLine(FontName);
            }
        }

        public string FontName
        {
            get => _fontName;
            set => this.RaiseAndSetIfChanged(ref _fontName, value);
        }

        private string _fontName;

        private FontItem _selectedItem;

        private readonly Node _root = new Node();

        private class Node
        {
            public ObservableCollection<FontItem> Children { get; } = new ObservableCollection<FontItem>();
            public void AddNewItem(FontItem fontItem) => Children.Add(fontItem);
        }

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
            foreach (var fileName in result)
            {
                await using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                var typeface = new WaterTrans.GlyphLoader.Typeface(fs);
                _root.AddNewItem(new FontItem {FileName = fileName, Typeface = typeface});
            }
        });

        #endregion
    }
}
