using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism.Services.Dialogs;

namespace Prism.Interactivity.InteractionRequest
{
    public class DialogInteractionRequest : InteractionRequest<DialogNotification, IDialogResult>, IDialogService
    {
        public void Show(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            ShowDialogInternal(name, parameters, callback, false);
        }

        public void Show(string name, IDialogParameters parameters, Action<IDialogResult> callback, string windowName)
        {
            ShowDialogInternal(name, parameters, callback, false, windowName);
        }

        public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            ShowDialogInternal(name, parameters, callback, true);
        }

        public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback, string windowName)
        {
            ShowDialogInternal(name, parameters, callback, true, windowName);
        }

        void ShowDialogInternal(string name, IDialogParameters parameters, Action<IDialogResult> callback, bool isModal, string windowName = null)
        {
            DialogNotification notification = new DialogNotification()
            {
                DialogName = name,
                Parameters = parameters,
                IsModal = isModal,
                WindowName = windowName
            };
            this.Raise(notification, callback);
        }
    }
}
