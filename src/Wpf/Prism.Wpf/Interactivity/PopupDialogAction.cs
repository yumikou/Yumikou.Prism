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
        /// <summary>
        /// DependencyProperty for <see cref="WindowStyle" /> property.
        /// </summary>
        public static readonly DependencyProperty WindowStyleProperty =
            DialogHost.WindowStyleProperty.AddOwner(typeof(PopupDialogAction));

        /// <summary>
        /// DependencyProperty for <see cref="Owner" /> property.
        /// </summary>
        public static readonly DependencyProperty OwnerProperty =
            DialogHost.OwnerProperty.AddOwner(typeof(PopupDialogAction));

        /// <summary>
        /// DependencyProperty for <see cref="IsOwnerEnabled" /> property.
        /// </summary>
        public static readonly DependencyProperty IsOwnerEnabledProperty =
            DialogHost.IsOwnerEnabledProperty.AddOwner(typeof(PopupDialogAction));

        public Style WindowStyle
        {
            get { return (Style)GetValue(WindowStyleProperty); }
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

        protected override void Invoke(object parameter)
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
                    else if (this.AssociatedObject is not null)
                    {
                        dialogServiceControl.Owner = Window.GetWindow(this.AssociatedObject);
                    }
                }
                dialogServiceControl.IsModal = args.Parameter.IsModal;
                dialogServiceControl.Closed += (s, o) =>
                {
                    args.Callback?.Invoke(o.DialogResult);
                };

                dialogServiceControl.Open(args.Parameter.IsBlocked);
            }
        }
    }
}
