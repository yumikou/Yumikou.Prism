using Prism.Ioc;
using Prism.Common;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace Prism.Services.Dialogs
{
    public class VirtualDialogHost: Control
    {
        private readonly IContainerExtension _containerExtension;
        private IVirtualDialogWindow _dialogWindow;
        private bool _isShowAsyncChangedEnabled = true;
        private DispatcherFrame _modalDialogDispatcherFrame;

        public event EventHandler<DialogResultEventArgs> Closed;



        public VirtualDialogHost()
        {

        }
    }
}
