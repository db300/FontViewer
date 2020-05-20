using System;
using System.Collections.Generic;
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

        public ReactiveCommand<Unit, Unit> OpenCommand => ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await new OpenFileDialog{Filters = {new FileDialogFilter{Name = ".otf",Extensions = new List<string>{ "otf" } }}}.ShowAsync((Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow);
            if (result?.Length > 0)
            {
                System.Diagnostics.Debug.WriteLine(result);
            }
        });

        public ReactiveCommand<Unit, Unit> ExitCommand => ReactiveCommand.Create(() => { (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Shutdown(); });

        #endregion
    }
}
