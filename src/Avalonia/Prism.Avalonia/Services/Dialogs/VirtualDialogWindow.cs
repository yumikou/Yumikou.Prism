using Avalonia.Controls;
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
           
        }

        public virtual void Show()
        {

        }
    }
}
