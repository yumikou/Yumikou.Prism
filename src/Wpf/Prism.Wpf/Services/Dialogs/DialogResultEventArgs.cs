using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Services.Dialogs
{
    public class DialogResultEventArgs : EventArgs
    {
        public DialogResultEventArgs() { }

        public DialogResultEventArgs(IDialogResult result)
        {
            DialogResult = result;
        }

        public IDialogResult DialogResult { get; set; }
    }
}
