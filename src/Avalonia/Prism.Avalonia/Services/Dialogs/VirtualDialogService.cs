using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;

namespace Prism.Services.Dialogs
{
    public class VirtualDialogService : IVirtualDialogService
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
            VirtualDialogHost dialogServiceControl = new VirtualDialogHost();
            dialogServiceControl.WindowName = windowName;
            dialogServiceControl.DialogName = name;
            dialogServiceControl.Parameters = parameters;
            dialogServiceControl.Owner = GetActiveOrMainTopLevel();
            dialogServiceControl.Closed += (sender, args) =>
            {
                tcs.SetResult(args.DialogResult);
            };
            dialogServiceControl.Open();
            return tcs.Task;
        }

        private static TopLevel GetActiveOrMainTopLevel()
        {
            TopLevel tl = null;
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime a1)
            {
                foreach (var item in a1.Windows)
                {
                    if (item.IsActive)
                    {
                        tl = item;
                        break;
                    }
                }
                if (tl is null)
                {
                    tl = a1.MainWindow;
                }
            }
            else if (Application.Current.ApplicationLifetime is ISingleViewApplicationLifetime sl)
            {
                tl = TopLevel.GetTopLevel(sl.MainView);
            }
            return tl;
        }
    }
}
