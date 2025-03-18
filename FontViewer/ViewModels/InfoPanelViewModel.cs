using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontViewer.ViewModels
{
    internal class InfoPanelViewModel : ViewModelBase
    {
        public InfoPanelViewModel()
        {
        }

        private string _someTextProperty;
        public string SomeTextProperty
        {
            get => _someTextProperty;
            set => this.RaiseAndSetIfChanged(ref _someTextProperty, value);
        }
    }
}
