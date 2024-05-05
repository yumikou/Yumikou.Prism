using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Interactivity.InteractionRequest;
using Prism.Services.Dialogs;
using Avalonia.Xaml.Interactivity;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Prism.Interactivity
{
    public class PopupVirtualDialogAction : AvaloniaObject, IAction
    {
        /// <summary>
        /// DependencyProperty for <see cref="VirtualWindowStyle" /> property.
        /// </summary>
        public static readonly StyledProperty<Style> VirtualWindowStyleProperty =
            Dialog.VirtualWindowStyleProperty.AddOwner<PopupVirtualDialogAction>();

        /// <summary>
        /// DependencyProperty for <see cref="Owner" /> property.
        /// </summary>
        public static readonly StyledProperty<Visual> OwnerProperty =
            VirtualDialogHost.OwnerProperty.AddOwner<PopupVirtualDialogAction>();

        /// <summary>
        /// DependencyProperty for <see cref="IsOwnerEnabled" /> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsOwnerEnabledProperty =
            VirtualDialogHost.IsOwnerEnabledProperty.AddOwner<PopupVirtualDialogAction>();

        public Style VirtualWindowStyle
        {
            get { return GetValue(VirtualWindowStyleProperty); }
            set { SetValue(VirtualWindowStyleProperty, value); }
        }

        public Visual Owner
        {
            get { return (Visual)GetValue(OwnerProperty); }
            set { SetValue(OwnerProperty, value); }
        }

        public bool IsOwnerEnabled
        {
            get { return (bool)GetValue(IsOwnerEnabledProperty); }
            set { SetValue(IsOwnerEnabledProperty, value); }
        }

        public object Execute(object sender, object parameter)
        {
            if (parameter is InteractionRequestedEventArgs<VirtualDialogNotification, IDialogResult> args && args.Parameter is not null)
            {
                VirtualDialogHost dialogServiceControl = new VirtualDialogHost();
                dialogServiceControl.WindowName = args.Parameter.WindowName;
                dialogServiceControl.DialogName = args.Parameter.DialogName;
                dialogServiceControl.Parameters = args.Parameter.Parameters;
                dialogServiceControl.VirtualWindowStyle = VirtualWindowStyle;
                if (IsOwnerEnabled)
                {
                    if (Owner is not null)
                    {
                        dialogServiceControl.Owner = Owner;
                    }
                    else
                    {
                        //dialogServiceControl.Owner= // avalonia的triggerAction无法获取AssociatedObject，因此无法自动查找所在的Window
                    }
                }
                dialogServiceControl.Closed += (s, o) =>
                {
                    args.Callback?.Invoke(o.DialogResult);
                };

                dialogServiceControl.Open();
                return true;
            }
            return false;
        }
    }
}
