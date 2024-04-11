using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Prism.Common;
using Prism.Ioc;

namespace Prism.Services.Dialogs
{
    /// <summary>
    /// Implements <see cref="IDialogService"/> to show modal and non-modal dialogs.
    /// </summary>
    /// <remarks>
    /// The dialog's ViewModel must implement IDialogAware.
    /// </remarks>
    public class DialogService : IDialogService
    {
        private readonly IContainerExtension _containerExtension;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService"/> class.
        /// </summary>
        /// <param name="containerExtension"></param>
        public DialogService(IContainerExtension containerExtension)
        {
            _containerExtension = containerExtension;
        }

        /// <summary>
        /// Shows a non-modal dialog.
        /// </summary>
        /// <param name="name">The name of the dialog to show.</param>
        /// <param name="parameters">The parameters to pass to the dialog.</param>
        /// <param name="callback">The action to perform when the dialog is closed.</param>
        public void Show(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            ShowDialogInternal(name, parameters, callback, false);
        }

        /// <summary>
        /// Shows a non-modal dialog.
        /// </summary>
        /// <param name="name">The name of the dialog to show.</param>
        /// <param name="parameters">The parameters to pass to the dialog.</param>
        /// <param name="callback">The action to perform when the dialog is closed.</param>
        /// <param name="windowName">The name of the hosting window registered with the IContainerRegistry.</param>
        public void Show(string name, IDialogParameters parameters, Action<IDialogResult> callback, string windowName)
        {
            ShowDialogInternal(name, parameters, callback, false, windowName);
        }

        /// <summary>
        /// Shows a modal dialog.
        /// </summary>
        /// <param name="name">The name of the dialog to show.</param>
        /// <param name="parameters">The parameters to pass to the dialog.</param>
        /// <param name="callback">The action to perform when the dialog is closed.</param>
        public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            ShowDialogInternal(name, parameters, callback, true);
        }

        /// <summary>
        /// Shows a modal dialog.
        /// </summary>
        /// <param name="name">The name of the dialog to show.</param>
        /// <param name="parameters">The parameters to pass to the dialog.</param>
        /// <param name="callback">The action to perform when the dialog is closed.</param>
        /// <param name="windowName">The name of the hosting window registered with the IContainerRegistry.</param>
        public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback, string windowName)
        {
            ShowDialogInternal(name, parameters, callback, true, windowName);
        }

        public Task<IDialogResult> ShowDialogAsync(string name, IDialogParameters parameters)
        {
            return ShowDialogInternalAsync(name, parameters, true);
        }

        public Task<IDialogResult> ShowDialogAsync(string name, IDialogParameters parameters, string windowName)
        {
            return ShowDialogInternalAsync(name, parameters, true, windowName);
        }

        void ShowDialogInternal(string name, IDialogParameters parameters, Action<IDialogResult> callback, bool isModal, string windowName = null)
        {
            DialogHost dialogServiceControl = new DialogHost();
            dialogServiceControl.WindowName = windowName;
            dialogServiceControl.DialogName = name;
            dialogServiceControl.Parameters = parameters;
            dialogServiceControl.Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
            dialogServiceControl.IsModal = isModal;
            dialogServiceControl.Closed += (sender, args) =>
            {
                callback?.Invoke(args.DialogResult);
            };
            dialogServiceControl.Open(true);
        }

        Task<IDialogResult> ShowDialogInternalAsync(string name, IDialogParameters parameters, bool isModal, string windowName = null)
        {
            TaskCompletionSource<IDialogResult> tcs = new TaskCompletionSource<IDialogResult>();
            DialogHost dialogServiceControl = new DialogHost();
            dialogServiceControl.WindowName = windowName;
            dialogServiceControl.DialogName = name;
            dialogServiceControl.Parameters = parameters;
            dialogServiceControl.Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
            dialogServiceControl.IsModal = isModal;
            dialogServiceControl.Closed += (sender, args) =>
            {
                tcs.SetResult(args.DialogResult);
            };
            dialogServiceControl.Open(false);
            return tcs.Task;
        }
    }
}
