﻿using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Prism.Services.Dialogs
{
    /// <summary>
    /// This class contains <see cref="IDialogWindow"/> and <see cref="IVirtualDialogWindow"/> attached properties.
    /// </summary>
    public class Dialog
    {
        /// <summary>Identifies the WindowStyle attached property.</summary>
        /// <remarks>This attached property is used to specify the style of a <see cref="IDialogWindow"/>.</remarks>
        public static readonly StyledProperty<Style> WindowStyleProperty =
            AvaloniaProperty.RegisterAttached<Dialog, AvaloniaObject, Style>("WindowStyle");

        /// <summary>Identifies the VirtualWindowStyle attached property.</summary>
        /// <remarks>This attached property is used to specify the style of a <see cref="IVirtualDialogWindow"/>.</remarks>
        public static readonly StyledProperty<Style> VirtualWindowStyleProperty =
            AvaloniaProperty.RegisterAttached<Dialog, AvaloniaObject, Style>("VirtualWindowStyle");

        /// <summary>Identifies the WindowStartupLocation attached property.</summary>
        /// <remarks>This attached property is used to specify the startup location of a <see cref="IDialogWindow"/>.</remarks>
        public static readonly AvaloniaProperty WindowStartupLocationProperty =
            AvaloniaProperty.RegisterAttached<AvaloniaObject, WindowStartupLocation>(
                name: "WindowStartupLocation",
                ownerType: typeof(Dialog));

        static Dialog()
        {
            WindowStartupLocationProperty.Changed.Subscribe(args => OnWindowStartupLocationChanged(args?.Sender, args));
        }

        /// <summary>
        /// Gets the value for the <see cref="WindowStyleProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <returns>The <see cref="WindowStyleProperty"/> attached to the <paramref name="obj"/> element.</returns>
        public static Style GetWindowStyle(AvaloniaObject obj)
        {
            return obj.GetValue(WindowStyleProperty);
        }

        /// <summary>
        /// Sets the <see cref="WindowStyleProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <param name="value">The Style to attach.</param>
        public static void SetWindowStyle(AvaloniaObject obj, Style value)
        {
            obj.SetValue(WindowStyleProperty, value);
        }

        /// <summary>
        /// Gets the value for the <see cref="VirtualWindowStyleProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <returns>The <see cref="VirtualWindowStyleProperty"/> attached to the <paramref name="obj"/> element.</returns>
        public static Style GetVirtualWindowStyle(AvaloniaObject obj)
        {
            return obj.GetValue(VirtualWindowStyleProperty);
        }

        /// <summary>
        /// Sets the <see cref="VirtualWindowStyleProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <param name="value">The Style to attach.</param>
        public static void SetVirtualWindowStyle(AvaloniaObject obj, Style value)
        {
            obj.SetValue(VirtualWindowStyleProperty, value);
        }

        /// <summary>
        /// Gets the value for the <see cref="WindowStartupLocationProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <returns>The <see cref="WindowStartupLocationProperty"/> attached to the <paramref name="obj"/> element.</returns>
        public static WindowStartupLocation GetWindowStartupLocation(AvaloniaObject obj)
        {
            return (WindowStartupLocation)obj.GetValue(WindowStartupLocationProperty);
        }

        /// <summary>
        /// Sets the <see cref="WindowStartupLocationProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <param name="value">The WindowStartupLocation to attach.</param>
        public static void SetWindowStartupLocation(AvaloniaObject obj, WindowStartupLocation value)
        {
            obj.SetValue(WindowStartupLocationProperty, value);
        }

        private static void OnWindowStartupLocationChanged(AvaloniaObject sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (sender is Window window)
                window.WindowStartupLocation = (WindowStartupLocation)e.NewValue;
        }
    }
}
