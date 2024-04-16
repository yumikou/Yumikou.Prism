using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Services.Dialogs
{
    public class VirtualDialogWindow : ContentControl, IVirtualDialogWindow
    {
        public virtual IDialogResult Result { get; set; }

        public event EventHandler Closed;
        public event EventHandler<VirtualWindowClosingEventArgs> Closing;
        public event EventHandler<EventArgs> Opened;

        public virtual void Close()
        {
            //先调用Closing
            //允许关闭后，将当前对象从OverlayLayer中移除，要等待动画执行完才能移除，因为从OverlayLayer移除时当前视图会变成不可见
            //然后调用Closed
        }

        public virtual void Show()
        {

        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
        }
    }
}
