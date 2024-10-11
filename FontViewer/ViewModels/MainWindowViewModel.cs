using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive;

namespace FontViewer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            _someTextProperty = string.Empty; // Initialize _someTextProperty to a non-null value
            OpenCommandClick = ReactiveCommand.Create(OnOpenCommandClick);
        }

        private string _someTextProperty;
        public string SomeTextProperty
        {
            get => _someTextProperty;
            set => this.RaiseAndSetIfChanged(ref _someTextProperty, value);
        }

        public ReactiveCommand<Unit, Unit> OpenCommandClick { get; }

        private async void OnOpenCommandClick()
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
            var files = await Helpers.CommonHelper.OpenFontFileAsync(desktop.MainWindow);
            if (!(files?.Count > 0)) return;

            var fontNames = new List<string>();
            foreach (var file in files)
            {
                var fontName = Helpers.SkiaSharpHelper.ReadFontNameTable(file.Path.LocalPath);
                fontNames.Add(fontName);
            }

            SomeTextProperty = string.Join("\n", fontNames);
        }
    }
}
