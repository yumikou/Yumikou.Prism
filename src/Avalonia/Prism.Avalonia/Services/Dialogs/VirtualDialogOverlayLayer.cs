using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Services.Dialogs
{
    public class VirtualDialogOverlayLayer : ContentControl
    {
        private IDisposable _rootBoundsWatcher;

        public VirtualDialogOverlayLayer()
        {
            //HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            //VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
        }

        protected override Type StyleKeyOverride => typeof(OverlayPopupHost);

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
