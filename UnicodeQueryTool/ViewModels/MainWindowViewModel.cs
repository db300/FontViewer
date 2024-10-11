using ReactiveUI;
using System.Windows.Input;

namespace UnicodeQueryTool.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            ConvertToUnicodeCommand = ReactiveCommand.Create(ConvertToUnicode);
            ConvertToTextCommand = ReactiveCommand.Create(ConvertToText);
        }

        private string _inputText = string.Empty;
        private string _inputUniocde = string.Empty;

        public string InputText
        {
            get => _inputText;
            set => this.RaiseAndSetIfChanged(ref _inputText, value);
        }

        public string InputUnicode
        {
            get => _inputUniocde;
            set => this.RaiseAndSetIfChanged(ref _inputUniocde, value);
        }

        public ICommand ConvertToUnicodeCommand { get; }
        public ICommand ConvertToTextCommand { get; }

        private void ConvertToUnicode()
        {
            InputUnicode = Helpers.UnicodeTextConverter.TextToUnicode(InputText);
        }

        private void ConvertToText()
        {
            InputText = Helpers.UnicodeTextConverter.UnicodeToText(InputUnicode);
        }
    }
}
