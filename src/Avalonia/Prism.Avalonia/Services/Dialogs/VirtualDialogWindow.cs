using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Rendering.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Services.Dialogs
{
    [PseudoClasses(":open", ":close")]
    public class VirtualDialogWindow : ContentControl, IVirtualDialogWindow
    {
        private VirtualDialogWindowMask _mask;

        #region Dependency Property

        /// <summary>
        /// DependencyProperty for <see cref="IsCloseAnimationCompleted" /> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsCloseAnimationCompletedProperty =
            AvaloniaProperty.Register<VirtualDialogWindow, bool>("IsCloseAnimationCompleted", false);

        /// <summary>
        /// DependencyProperty for <see cref="IsAutoCloseByMaskTapped" /> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsAutoCloseByMaskTappedProperty =
            AvaloniaProperty.Register<VirtualDialogWindow, bool>("IsAutoCloseByMaskTapped", false);

        /// <summary>
        /// DependencyProperty for <see cref="MaskBackground" /> property.
        /// </summary>
        public static readonly StyledProperty<IBrush?> MaskBackgroundProperty =
            AvaloniaProperty.Register<VirtualDialogWindow, IBrush?>("MaskBackground", null);

        #endregion

        public bool IsCloseAnimationCompleted
        {
            get { return (bool)GetValue(IsCloseAnimationCompletedProperty); }
            set { SetValue(IsCloseAnimationCompletedProperty, value); }
        }

        public bool IsAutoCloseByMaskTapped
        {
            get { return (bool)GetValue(IsAutoCloseByMaskTappedProperty); }
            set { SetValue(IsAutoCloseByMaskTappedProperty, value); }
        }

        public IBrush? MaskBackground
        {
            get { return (IBrush?)GetValue(MaskBackgroundProperty); }
            set { SetValue(MaskBackgroundProperty, value); }
        }

        public virtual IDialogResult Result { get; set; }
        public Visual Owner { get; set; }

        public event EventHandler Closed;
        public event EventHandler<VirtualWindowClosingEventArgs> Closing;
        public event EventHandler<EventArgs> Opened;

        public virtual void Close()
        {
            var cancelArgs = new VirtualWindowClosingEventArgs();
            RaiseClosing(cancelArgs);
            if (cancelArgs.Cancel) return; // 取消关闭

            IsCloseAnimationCompleted = false;
            IDisposable isCompletedWatcher = null;
            isCompletedWatcher = IsCloseAnimationCompletedProperty.Changed.Subscribe((args) => {
                var isCompleted = args.NewValue.Value;
                if (!isCompleted) throw new Exception("IsCloseAnimationCompleted通过动画只能修改为True");

                var ol = OverlayLayer.GetOverlayLayer(_mask);
                if (ol == null) throw new InvalidOperationException("OverlayLayer not found! The dialog may have been closed");
                ol.Children.Remove(_mask);
                _mask.Content = null;
                RaiseClosed(new EventArgs());

                isCompletedWatcher?.Dispose();
                isCompletedWatcher = null;
            });
            _mask?.Hidden();
            PseudoClasses.Set(":open", false);
            PseudoClasses.Set(":close", true);
        }

        public virtual void Open()
        {
            if (Owner is null) throw new InvalidOperationException("Owner not found!");
            OverlayLayer ol = OverlayLayer.GetOverlayLayer(Owner);
            if (ol is null) throw new InvalidOperationException("OverlayLayer not found from Owner!");

            this.Loaded += VirtualDialogWindow_Loaded;
            if (_mask is null)
            {
                _mask = new VirtualDialogWindowMask();
                _mask.BackgroundTapped += Mask_BackgroundTapped;
            }
            _mask.Content = this;
            ol.Children.Add(_mask);

            PseudoClasses.Set(":close", false);
            PseudoClasses.Set(":open", true);
        }

        private void Mask_BackgroundTapped(object sender, EventArgs e)
        {
            if (IsAutoCloseByMaskTapped)
            {
                Close();
            }
        }

        private void VirtualDialogWindow_Loaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.Loaded -= VirtualDialogWindow_Loaded;

            RaiseOpened();
        }

        protected virtual void RaiseOpened()
        { 
            Opened?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void RaiseClosing(VirtualWindowClosingEventArgs args)
        {
            Closing?.Invoke(this, args);
        }

        protected virtual void RaiseClosed(EventArgs args)
        { 
            Closed?.Invoke(this, args);
        }
    }
}
