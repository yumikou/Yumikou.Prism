using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Prism.Interactivity.InteractionRequest;
using Prism.Services.Dialogs;
using System.Xml.Linq;
#if NET40
using System.Windows.Interactivity;
#else
using Microsoft.Xaml.Behaviors;
#endif

namespace Prism.Interactivity
{
    public class PopupDialogAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            if (parameter is InteractionRequestedEventArgs<DialogNotification, IDialogResult> args && args.Parameter is not null)
            {
                DialogServiceControl dialogServiceControl = new DialogServiceControl();
                dialogServiceControl.WindowName = args.Parameter.WindowName;
                dialogServiceControl.DialogName = args.Parameter.DialogName;
                dialogServiceControl.Parameters = args.Parameter.Parameters;
                if (this.AssociatedObject is not null)
                {
                    dialogServiceControl.Owner = Window.GetWindow(this.AssociatedObject);
                }
                dialogServiceControl.IsModal = args.Parameter.IsModal;
                dialogServiceControl.Closed += (s, o) =>
                {
                    args.Callback?.Invoke(o.DialogResult);
                };

                dialogServiceControl.IsShow = true;
            }
        }
    }
}
