using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using SkiaSharp;
using System.IO;
using System.Threading.Tasks;

namespace FontPreviewer
{
    public partial class MainWindow : Window
    {
        #region constructor
        public MainWindow()
        {
            InitializeComponent();

            _imageControl = new Image();
            GlyphPreviewCanvas.Children.Add(_imageControl);
        }
        #endregion

        #region property
        private Image _imageControl;
        private iHawkSkiaSharpCommonLibrary.Helpers.SkiaSharpHelper? _helper;
        private int _textSize = 200;
        private const string DefaultText = "用心绽放文字之美 Abc123";
        private Avalonia.Media.Imaging.Bitmap? _cachedBitmap;
        #endregion

        #region method
        private async Task DrawCanvasAsync()
        {
            if (_helper is null) return;

            var param = new iHawkSkiaSharpCommonLibrary.Entities.GlyphPreviewParam
            {
                YMinMaxVisible = YMinMaxVisibleCheckBox.IsChecked ?? false,
                HheaAscDescVisible = HheaAscDescVisibleCheckBox.IsChecked ?? false,
                TypoAscDescVisible = TypoAscDescVisibleCheckBox.IsChecked ?? false,
                WinAscDescVisible = WinAscDescVisibleCheckBox.IsChecked ?? false
            };

            var text = InputTextBox.Text ?? DefaultText;
            var img = await Task.Run(() => _helper.DrawTextToImage(text, (int)GlyphPreviewCanvas.Bounds.Width, (int)GlyphPreviewCanvas.Bounds.Height, SKColors.White, SKColors.Black, _textSize, param));

            using var data = img.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = new MemoryStream();
            data?.SaveTo(stream);
            stream.Seek(0, SeekOrigin.Begin);

            var bitmap = new Avalonia.Media.Imaging.Bitmap(stream);
            _imageControl.Source = bitmap;
        }
        #endregion

        #region event handler
        private async void OpenCommandClick(object sender, RoutedEventArgs e)
        {
            var files = await iHawkAvaloniaCommonLibrary.CommonHelper.OpenFontFileAsync(this);
            if (!(files?.Count > 0)) return;

            _helper = new iHawkSkiaSharpCommonLibrary.Helpers.SkiaSharpHelper(files[0].Path.LocalPath);
            var s = _helper.GetTypeSetting();
            InfoBlock.Text = s;

            await DrawCanvasAsync();
        }

        private async void CheckBoxChanged(object? sender, RoutedEventArgs e)
        {
            await DrawCanvasAsync();
        }

        private async void GlyphPreviewCanvas_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            await DrawCanvasAsync();
        }

        private async void GlyphPreviewCanvas_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            _textSize += (int)e.Delta.Y * 10; // 调整文本大小的增量
            if (_textSize < 10) _textSize = 10; // 确保文本大小不小于10
            await DrawCanvasAsync();
        }

        private async void InputTextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await DrawCanvasAsync();
            }
        }
        #endregion
    }
}