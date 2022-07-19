using System;
using System.Numerics;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using SkiaGame.Events;
using SkiaGame.Info;
using SkiaGame.Input;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using Keyboard = System.Windows.Input.Keyboard;
using MouseButton = SkiaGame.Input.MouseButton;

namespace SkiaGame.Wpf
{
    public class SkiaGameForm : SKElement
    {
        public static readonly DependencyProperty EngineProperty =
            DependencyProperty.Register(name: nameof(Engine), propertyType: typeof(Engine),
                ownerType: typeof(SkiaGameForm),
                typeMetadata: new FrameworkPropertyMetadata(EngineChangedCallBack));

        private static void EngineChangedCallBack(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            if (sender is SkiaGameForm form)
            {
                form.EngineReinit();
            }
        }

        public Engine Engine
        {
            get => (Engine)GetValue(EngineProperty);
            set => SetValue(EngineProperty, value);
        }

        private readonly Timer _timer = new();
        private bool _initialized;

        private ScreenInfo _screenInfo = ScreenInfo.Zero;
        public SkiaGameForm() { Loaded += (_, _) => Keyboard.Focus(this); }

        // ReSharper disable once UnusedMember.Global
        public SkiaGameForm(Engine engine) { Engine = engine; }

        /// <summary>
        /// A Pessoa não deveria Reatribuir a Engine em Runtime, isso não é muito previsível e suportado.
        /// </summary>
        public void EngineReinit()
        {
            _timer.Interval = 1000.0f / Engine.FrameRate;
            if (!_initialized)
            {
                _timer.AutoReset = true;
                _timer.Elapsed += FrameRateTimer;
                _initialized = true;
            } else
            {
                Engine.Platform.IsWpf = true;
                Engine.Platform.IsDesktop = true;
                Engine.InternalSetScreenInfo(_screenInfo);
                Engine.InternalExecuteOnStart();
            }
        }

        private void UpdateScreenInfo()
        {
            var screenSize = CanvasSize;
            var width = screenSize.Width;
            var height = screenSize.Height;
            var orientation = height > width ? Orientation.Portrait : Orientation.Landscape;
            var size = new SKSize(width, height);
            var density = 1.0f;
            var source = PresentationSource.FromVisual(this);
            if (source is { CompositionTarget: { } })
            {
                density = (float)source.CompositionTarget.TransformToDevice.M11;
            }

            _screenInfo = new ScreenInfo(size, orientation, density);
        }

        private void FrameRateTimer(object? sender, ElapsedEventArgs e)
        {
            Application.Current?.Dispatcher.Invoke(InvalidateVisual);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            UpdateScreenInfo();
            Engine.InternalSetScreenInfo(_screenInfo);
            Engine.InternalExecuteOnStart();
            _timer.Start();
            base.OnRenderSizeChanged(sizeInfo);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            var keyStr = e.Key.ToString();
            var result = Enum.TryParse(keyStr, out KeyCode keyCode);
            if (!result) return;
            var eventArgs = new SkKeyPressEventArgs(keyCode);
            Engine.InternalKeyPress(eventArgs);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            var keyStr = e.Key.ToString();
            var result = Enum.TryParse(keyStr, out KeyCode keyCode);
            if (!result) return;
            var eventArgs = new SkKeyPressEventArgs(keyCode);
            Engine.InternalKeyRelease(eventArgs);
        }

        private Vector2 TransformPosition(Point point)
        {
            var source = PresentationSource.FromVisual(this);

            if (source is not { CompositionTarget: { } }) return Vector2.Zero;
            var m11 = source.CompositionTarget.TransformToDevice.M11;
            var m22 = source.CompositionTarget.TransformToDevice.M22;

            return new Vector2((float)(point.X * m11), (float)(point.Y * m22));
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            var res = Enum.TryParse(e.ChangedButton.ToString(), out MouseButton but);
            if (!res) return;
            if (Engine.Mouse[but].IsPressed) return;
            var position = TransformPosition(e.GetPosition(this));
            var evArgs = SetMouseState(but, position, true);

            Engine.InternalTouchPress(evArgs);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            MouseButton button;
            if (Engine.Mouse[MouseButton.Left].IsPressed &&
                e.LeftButton == MouseButtonState.Released)
            {
                button = MouseButton.Left;
            } else if (Engine.Mouse[MouseButton.Middle].IsPressed &&
                       e.MiddleButton == MouseButtonState.Released)
            {
                button = MouseButton.Middle;
            } else if (Engine.Mouse[MouseButton.Right].IsPressed &&
                       e.RightButton == MouseButtonState.Released)
            {
                button = MouseButton.Right;
            } else
                return;

            var position = TransformPosition(e.GetPosition(this));
            var evArgs = SetMouseState(button, position, false);
            Engine.InternalTouchRelease(evArgs);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            var position = TransformPosition(e.GetPosition(this));
            Engine.InternalUpdateMouseDesktop(position);
        }

        private SkTouchEventArgs SetMouseState(MouseButton button, Vector2 position, bool state)
        {
            var eventArgs = new SkTouchEventArgs(position);
            var mouseInfo = new MouseBase(button, position, state);
            Engine.InternalSetMouseState(mouseInfo);
            return eventArgs;
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);
            var eventArgs = new PaintEventArgs(e.Info, e.Surface);
            Engine.InternalPaintSurface(eventArgs);
        }
    }
}