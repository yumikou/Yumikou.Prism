using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Services.Dialogs
{
    [TemplatePart(Name = "PART_Background", Type = typeof(Border), IsRequired = true)]
    [PseudoClasses(":show", ":hidden")]
    public class VirtualDialogWindowMask : ContentControl
    {
        private IDisposable _rootBoundsWatcher;

        public event EventHandler<EventArgs> BackgroundTapped;

        public virtual void Show()
        {
            PseudoClasses.Set(":hidden", false);
            PseudoClasses.Set(":show", true);
        }

        public virtual void Hidden()
        {
            PseudoClasses.Set(":show", false);
            PseudoClasses.Set(":hidden", true);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            var bg = e.NameScope.Find("PART_Background") as Border;
            bg.Tapped += Background_Tapped;
        }

        private void Background_Tapped(object sender, Avalonia.Input.TappedEventArgs e)
        {
            // 也许拦截ContentPresenter的Tapped事件会更好，但我不想频繁的触发Tapped事件
            BackgroundTapped?.Invoke(this, EventArgs.Empty);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _ = base.MeasureOverride(availableSize);

            if (VisualRoot is TopLevel tl)
            {
                return tl.ClientSize;
            }
            else if (VisualRoot is Control c)
            {
                return c.Bounds.Size;
            }

            return default;
        }


        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            if (e.Root is Control wb)
            {
                // OverlayLayer is a Canvas, so we won't get a signal to resize if the window
                // bounds change. Subscribe to force update
                _rootBoundsWatcher = wb.GetObservable(BoundsProperty).Subscribe(_ => OnRootBoundsChanged());
            }
            Show();
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            _rootBoundsWatcher?.Dispose();
            _rootBoundsWatcher = null;
        }

        private void OnRootBoundsChanged()
        {
            InvalidateMeasure();
        }
    }
}
