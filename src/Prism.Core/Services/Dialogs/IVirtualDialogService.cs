using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Services.Dialogs
{
    public interface IVirtualDialogService
    {
        /// <summary>
        /// Shows a modal dialog.
        /// </summary>
        /// <param name="name">The name of the dialog to show.</param>
        /// <param name="parameters">The parameters to pass to the dialog.</param>
        /// <param name="callback">The action to perform when the dialog is closed.</param>
        Task<IDialogResult> ShowDialogAsync(string name, IDialogParameters parameters);


        /// <summary>
        /// Shows a modal dialog.
        /// </summary>
        /// <param name="name">The name of the dialog to show.</param>
        /// <param name="parameters">The parameters to pass to the dialog.</param>
        /// <param name="callback">The action to perform when the dialog is closed.</param>
        /// <param name="windowName">The name of the hosting window registered with the IContainerRegistry.</param>
        Task<IDialogResult> ShowDialogAsync(string name, IDialogParameters parameters, string windowName);
    }
}
