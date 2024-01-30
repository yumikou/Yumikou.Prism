using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism.Services.Dialogs;

namespace Prism.Interactivity.InteractionRequest
{
    public class DialogNotification
    {
        public string DialogName { get; set; }

        public IDialogParameters Parameters { get; set; }

        public bool IsModal { get; set; }

        public string WindowName { get; set; }
    }
}
