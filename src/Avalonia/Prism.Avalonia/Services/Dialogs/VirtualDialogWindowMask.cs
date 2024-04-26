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
    [PseudoClasses(":show", ":hidden")]
    public class VirtualDialogWindowMask : ContentControl
    {
        private IDisposable _rootBoundsWatcher;

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
