using System;
using System.Collections.Generic;
using System.Text;
using Prism.Services.Dialogs;

namespace Prism.Interactivity.InteractionRequest
{
    public class VirtualDialogNotification
    {
        public string DialogName { get; set; }

        public IDialogParameters Parameters { get; set; }

        public string WindowName { get; set; }
    }
}
