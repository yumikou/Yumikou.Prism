using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Services.Dialogs
{
    internal static class IVirtualDialogWindowExtensions
    {
        internal static IDialogAware GetDialogViewModel(this IVirtualDialogWindow dialogWindow)
        {
            return (IDialogAware)dialogWindow.DataContext;
        }
    }
}
