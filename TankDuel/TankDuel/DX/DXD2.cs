using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.WIC;
using SharpDX.Windows;

namespace DX
{
    class DX2D : IDisposable
    {
        private SharpDX.Direct2D1.Factory _factory;
        public SharpDX.Direct2D1.Factory Factory { get => _factory; }

        private SharpDX.DirectWrite.Factory _writeFactory;
        public SharpDX.DirectWrite.Factory WriteFactory { get => _writeFactory; }

        private WindowRenderTarget _renderTarget;
        public WindowRenderTarget RenderTarget { get => _renderTarget; }

        private ImagingFactory _imagingFactory;
        public ImagingFactory ImagingFactory { get => _imagingFactory; }

        private TextFormat _textFormatUserInterface;
        public TextFormat TextFormatUserInterface { get => _textFormatUserInterface; }

        private TextFormat _textFormatStatistic;
        public TextFormat TextFormatStatistic { get => _textFormatStatistic; }

        private TextFormat _textFormatLastScreen;
        public TextFormat TextFormatLastScreen { get => _textFormatLastScreen; }

        private TextFormat _textFormatMenuButton;
        public TextFormat TextFormatMenuButton { get => _textFormatMenuButton; }

        private Brush _greenBrush;
        public Brush GreenBrush { get => _greenBrush; }

        private Brush _whiteBrush;
        public Brush WhiteBrush { get => _whiteBrush; }

        private Brush _blackBrush;
        public Brush BlackBrush { get => _blackBrush; }

        private Brush _redBrush;
        public Brush RedBrush { get => _redBrush; }

        private Brush _transparentBrush;
        public Brush TransparentBrush { get => _transparentBrush; }

        private List<SharpDX.Direct2D1.Bitmap> _bitmaps;
        public List<SharpDX.Direct2D1.Bitmap> Bitmaps { get => _bitmaps; }

        public DX2D(RenderForm form, int width = 1000, int height = 800)
        {
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            form.Size = new System.Drawing.Size(width, height);
            form.AllowUserResizing = false;

            _factory = new SharpDX.Direct2D1.Factory();
            _writeFactory = new SharpDX.DirectWrite.Factory();

            RenderTargetProperties renderProp = new RenderTargetProperties()
            {
                DpiX = 0,
                DpiY = 0,
                MinLevel = FeatureLevel.Level_10,
                PixelFormat = new SharpDX.Direct2D1.PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied),
                Type = RenderTargetType.Hardware,
                Usage = RenderTargetUsage.None
            };
           HwndRenderTargetProperties winProp = new HwndRenderTargetProperties()
            {
                Hwnd = form.Handle,
                PixelSize = new Size2(form.ClientSize.Width, form.ClientSize.Height),
                PresentOptions = PresentOptions.None                                      
            };
            _renderTarget = new WindowRenderTarget(_factory, renderProp, winProp)
            {
                AntialiasMode = AntialiasMode.PerPrimitive,
                TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode.Cleartype
            };

            _imagingFactory = new ImagingFactory();

            _textFormatUserInterface = new TextFormat(_writeFactory, "Castellar", 20)
            {
                ParagraphAlignment = ParagraphAlignment.Near,
                TextAlignment = TextAlignment.Center
            };
            _textFormatStatistic = new TextFormat(_writeFactory, "Calibri", 18)
            {
                ParagraphAlignment = ParagraphAlignment.Near,
                TextAlignment = TextAlignment.Leading
            };
            _textFormatMenuButton = new TextFormat(_writeFactory, "Castellar", 18)
            {
                ParagraphAlignment = ParagraphAlignment.Center,
                TextAlignment = TextAlignment.Center
            };
            _textFormatLastScreen = new TextFormat(_writeFactory, "Castellar", 100)
            {
                ParagraphAlignment = ParagraphAlignment.Center,
                TextAlignment = TextAlignment.Center
            };
            _greenBrush = new SolidColorBrush(_renderTarget, new Color(0f, 255f, 0f, 1f));
            _whiteBrush = new SolidColorBrush(_renderTarget, Color.White);
            _blackBrush = new SolidColorBrush(_renderTarget, Color.Black);
            _redBrush = new SolidColorBrush(_renderTarget, new Color(255f, 0f, 0f, 1f));
            _transparentBrush = new SolidColorBrush(_renderTarget, new Color(0f, 0f, 0f, 0.5f));
        }

        public int LoadBitmap(string imageFileName)
        {
            // Декодер формата
            BitmapDecoder decoder = new BitmapDecoder(_imagingFactory, imageFileName, DecodeOptions.CacheOnDemand);
            // Берем первый фрейм
            BitmapFrameDecode frame = decoder.GetFrame(0);
            // Также нужен конвертер формата 
            FormatConverter converter = new FormatConverter(_imagingFactory);
            converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppPRGBA, BitmapDitherType.Ordered4x4, null, 0.0, BitmapPaletteType.Custom);
            // Вот теперь можно и bitmap
            SharpDX.Direct2D1.Bitmap bitmap = SharpDX.Direct2D1.Bitmap.FromWicBitmap(_renderTarget, converter);

            // Освобождаем неуправляемые ресурсы
            Utilities.Dispose(ref converter);
            Utilities.Dispose(ref frame);
            Utilities.Dispose(ref decoder);

            // Добавляем изображение в коллекцию
            if (_bitmaps == null) _bitmaps = new List<SharpDX.Direct2D1.Bitmap>(4);
            _bitmaps.Add(bitmap);
            return _bitmaps.Count - 1;
        }

        public void Dispose()
        {
            if (_bitmaps != null)
            {
                for (int i = _bitmaps.Count - 1; i >= 0; i--) 
                {
                    SharpDX.Direct2D1.Bitmap bitmap = _bitmaps[i];
                    _bitmaps.RemoveAt(i);
                    Utilities.Dispose(ref bitmap);
                }
            }
            Utilities.Dispose(ref _whiteBrush);
            Utilities.Dispose(ref _greenBrush);
            Utilities.Dispose(ref _blackBrush);
            Utilities.Dispose(ref _redBrush);
            Utilities.Dispose(ref _transparentBrush);
            Utilities.Dispose(ref _textFormatUserInterface);
            Utilities.Dispose(ref _textFormatStatistic);
            Utilities.Dispose(ref _textFormatLastScreen);
            Utilities.Dispose(ref _textFormatMenuButton);
            Utilities.Dispose(ref _imagingFactory);
            Utilities.Dispose(ref _renderTarget);
            Utilities.Dispose(ref _factory);
        }
    }
}
