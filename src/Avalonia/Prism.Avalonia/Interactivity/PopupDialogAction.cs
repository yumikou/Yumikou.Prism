﻿using Prism.Interactivity.InteractionRequest;
using Prism.Services.Dialogs;
using System.Xml.Linq;
using Avalonia.Xaml.Interactivity;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Prism.Interactivity
{
    public class PopupDialogAction : AvaloniaObject, IAction
    {
        /// <summary>
        /// DependencyProperty for <see cref="WindowStyle" /> property.
        /// </summary>
        public static readonly StyledProperty<Style> WindowStyleProperty =
            DialogHost.WindowStyleProperty.AddOwner<PopupDialogAction>();

        /// <summary>
        /// DependencyProperty for <see cref="Owner" /> property.
        /// </summary>
        public static readonly StyledProperty<Window> OwnerProperty =
            DialogHost.OwnerProperty.AddOwner<PopupDialogAction>();

        /// <summary>
        /// DependencyProperty for <see cref="IsOwnerEnabled" /> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsOwnerEnabledProperty =
            DialogHost.IsOwnerEnabledProperty.AddOwner<PopupDialogAction>();

        public Style WindowStyle
        {
            get { return GetValue(WindowStyleProperty); }
            set { SetValue(WindowStyleProperty, value); }
        }

        public Window Owner
        {
            get { return (Window)GetValue(OwnerProperty); }
            set { SetValue(OwnerProperty, value); }
        }

        public bool IsOwnerEnabled
        {
            get { return (bool)GetValue(IsOwnerEnabledProperty); }
            set { SetValue(IsOwnerEnabledProperty, value); }
        }

        public object Execute(object sender, object parameter)
        {
            if (parameter is InteractionRequestedEventArgs<DialogNotification, IDialogResult> args && args.Parameter is not null)
            {
                DialogHost dialogServiceControl = new DialogHost();
                dialogServiceControl.WindowName = args.Parameter.WindowName;
                dialogServiceControl.DialogName = args.Parameter.DialogName;
                dialogServiceControl.Parameters = args.Parameter.Parameters;
                dialogServiceControl.WindowStyle = WindowStyle;
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
                dialogServiceControl.IsModal = args.Parameter.IsModal;
                dialogServiceControl.Closed += (s, o) =>
                {
                    args.Callback?.Invoke(o.DialogResult);
                };

                dialogServiceControl.Open(args.Parameter.IsBlocked);
                return true;
            }
            return false;
        }
    }
}
