using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            DialogNotification notification = new DialogNotification()
            {
                DialogName = name,
                Parameters = parameters,
                IsModal = isModal,
                IsBlocked = true,
                WindowName = windowName
            };
            this.Raise(notification, callback);
        }

        Task<IDialogResult> ShowDialogInternalAsync(string name, IDialogParameters parameters, bool isModal, string windowName = null)
        {
            TaskCompletionSource<IDialogResult> tcs = new TaskCompletionSource<IDialogResult>();
            DialogNotification notification = new DialogNotification()
            {
                DialogName = name,
                Parameters = parameters,
                IsModal = isModal,
                IsBlocked = false,
                WindowName = windowName
            };
            this.Raise(notification, result => {
                tcs.SetResult(result);
            });
            return tcs.Task;
        }
    }
}
