using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
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

            // 获取程序集版本号
            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0.0";
            // 设置窗口标题
            Title = $"FontPreviewer - {version}";
        }
        #endregion

        #region property
        private const int DefaultRenderDelayMilliseconds = 100;
        private const int ResizeRenderDelayMilliseconds = 150;
        private const int ImmediateRenderDelayMilliseconds = 0;

        private Image _imageControl;
        private iHawkSkiaSharpCommonLibrary.Helpers.SkiaSharpHelper? _helper;
        private int _textSize = 200;
        private const string DefaultText = "用心绽放文字之美\r\nAbc\r\n123";
        private Bitmap? _cachedBitmap;
        private CancellationTokenSource? _renderCancellationTokenSource;
        #endregion

        #region method
        private async Task DrawCanvasAsync(CancellationToken cancellationToken = default)
        {
            if (_helper is null) return;

            var width = (int)GlyphPreviewCanvas.Bounds.Width;
            var height = (int)GlyphPreviewCanvas.Bounds.Height;
            if (width <= 0 || height <= 0) return;

            var param = new iHawkSkiaSharpCommonLibrary.Entities.GlyphPreviewParam
            {
                YMinMaxVisible = YMinMaxVisibleCheckBox.IsChecked ?? false,
                HheaAscDescVisible = HheaAscDescVisibleCheckBox.IsChecked ?? false,
                TypoAscDescVisible = TypoAscDescVisibleCheckBox.IsChecked ?? false,
                WinAscDescVisible = WinAscDescVisibleCheckBox.IsChecked ?? false,
                LineGapTag = LineGapTagListBox.SelectedIndex == 0 ? "YMinMax" : "AscDesc"
            };

            var text = string.IsNullOrWhiteSpace(InputTextBox.Text) ? DefaultText : InputTextBox.Text;
            var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();

            using var img = await Task.Run(() => _helper.DrawTextToImage(lines, width, height, SKColors.White, SKColors.Black, _textSize, param), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var bitmap = CreateBitmap(img);
            var oldBitmap = _cachedBitmap;
            _cachedBitmap = bitmap;
            _imageControl.Source = bitmap;
            oldBitmap?.Dispose();
        }

        private static WriteableBitmap CreateBitmap(SKBitmap img)
        {
            var bitmap = new WriteableBitmap(
                new PixelSize(img.Width, img.Height),
                new Vector(96, 96),
                PixelFormat.Bgra8888,
                AlphaFormat.Premul);

            using var framebuffer = bitmap.Lock();
            var bytes = img.Bytes;
            if (framebuffer.RowBytes == img.RowBytes)
            {
                Marshal.Copy(bytes, 0, framebuffer.Address, bytes.Length);
                return bitmap;
            }

            for (var row = 0; row < img.Height; row++)
            {
                Marshal.Copy(
                    bytes,
                    row * img.RowBytes,
                    IntPtr.Add(framebuffer.Address, row * framebuffer.RowBytes),
                    img.RowBytes);
            }

            return bitmap;
        }

        private void RequestRender(int delayMilliseconds = DefaultRenderDelayMilliseconds)
        {
            CancelPendingRender();
            _renderCancellationTokenSource = new CancellationTokenSource();
            _ = ScheduleRenderAsync(delayMilliseconds, _renderCancellationTokenSource.Token);
        }

        private async Task ScheduleRenderAsync(int delayMilliseconds, CancellationToken cancellationToken)
        {
            try
            {
                if (delayMilliseconds > 0)
                {
                    await Task.Delay(delayMilliseconds, cancellationToken);
                }

                await DrawCanvasAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                TypeParamBlock.Text = $"预览失败：{ex.Message}";
            }
        }

        private void CancelPendingRender()
        {
            _renderCancellationTokenSource?.Cancel();
            _renderCancellationTokenSource?.Dispose();
            _renderCancellationTokenSource = null;
        }

        private void ClearCurrentBitmap()
        {
            _imageControl.Source = null;
            _cachedBitmap?.Dispose();
            _cachedBitmap = null;
        }
        #endregion

        #region event handler
        private async void OpenCommandClick(object sender, RoutedEventArgs e)
        {
            var files = await iHawkAvaloniaCommonLibrary.CommonHelper.OpenFontFileAsync(this);
            if (!(files?.Count > 0)) return;

            try
            {
                var filePath = files[0].Path.LocalPath;
                var helper = new iHawkSkiaSharpCommonLibrary.Helpers.SkiaSharpHelper(filePath);
                var s = helper.GetTypeSetting();

                _helper = helper;
                OpennedFileName.Text = filePath;
                TypeParamBlock.Text = s;

                RequestRender(ImmediateRenderDelayMilliseconds);
            }
            catch (Exception ex)
            {
                _helper = null;
                CancelPendingRender();
                ClearCurrentBitmap();
                OpennedFileName.Text = string.Empty;
                TypeParamBlock.Text = $"打开字体失败：{ex.Message}";
            }
        }

        private void CheckBoxChanged(object? sender, RoutedEventArgs e)
        {
            RequestRender(ImmediateRenderDelayMilliseconds);
        }

        private void ListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            RequestRender(ImmediateRenderDelayMilliseconds);
        }

        private void GlyphPreviewCanvas_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            RequestRender(ResizeRenderDelayMilliseconds);
        }

        private void GlyphPreviewCanvas_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            _textSize += (int)e.Delta.Y * 10; // 调整文本大小的增量
            if (_textSize < 10) _textSize = 10; // 确保文本大小不小于10
            FontSizeSlider.Value = _textSize;
            RequestRender(DefaultRenderDelayMilliseconds);
        }

        private void InputTextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RequestRender(ImmediateRenderDelayMilliseconds);
            }
        }

        private void FontSizeSlider_ValueChanged(object? sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            var newSize = (int)e.NewValue;
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"调整后的字号：{newSize}");
#endif
            if (_textSize == newSize) return;
            _textSize = newSize;
            RequestRender(DefaultRenderDelayMilliseconds);
        }

        protected override void OnClosed(EventArgs e)
        {
            CancelPendingRender();
            ClearCurrentBitmap();
            _helper = null;
            base.OnClosed(e);
        }
        #endregion
    }
}