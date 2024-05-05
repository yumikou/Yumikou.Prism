using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Prism.Services.Dialogs;

namespace Prism.Interactivity.InteractionRequest
{
    public class VirtualDialogInteractionRequest : InteractionRequest<VirtualDialogNotification, IDialogResult>, IVirtualDialogService
    {
        public Task<IDialogResult> ShowDialogAsync(string name, IDialogParameters parameters)
        {
            return ShowDialogInternalAsync(name, parameters);
        }

        public Task<IDialogResult> ShowDialogAsync(string name, IDialogParameters parameters, string windowName)
        {
            return ShowDialogInternalAsync(name, parameters, windowName);
        }

        Task<IDialogResult> ShowDialogInternalAsync(string name, IDialogParameters parameters, string windowName = null)
        {
            TaskCompletionSource<IDialogResult> tcs = new TaskCompletionSource<IDialogResult>();
            VirtualDialogNotification notification = new VirtualDialogNotification()
            {
                DialogName = name,
                Parameters = parameters,
                WindowName = windowName
            };
            this.Raise(notification, result => {
                tcs.SetResult(result);
            });
            return tcs.Task;
        }
    }
}
