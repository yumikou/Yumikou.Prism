
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using System;

namespace Prism.Services.Dialogs
{
    public interface IVirtualDialogWindow
    {
        /// <summary>
        /// Dialog content.
        /// </summary>
        object ContentArea { get; set; }

        /// <summary>
        /// Close the window.
        /// </summary>
        void Close();

        /// <summary>
        /// Show a dialog async.
        /// </summary>
        void Show();

        /// <summary>
        /// The data context of the window.
        /// </summary>
        /// <remarks>
        /// The data context must implement <see cref="IDialogAware"/>.
        /// </remarks>
        object DataContext { get; set; }

        /// <summary>
        /// Called when the window is loaded.
        /// </summary>
        event EventHandler<RoutedEventArgs> Loaded;

        /// <summary>
        /// Called when the window is closed.
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Called when the window is closing.
        /// </summary>
        // WPF: event CancelEventHandler Closing;
        // Ava: ...
        event EventHandler<WindowClosingEventArgs>? Closing; //TODO: 自定义VirtualWindowClosingEventArgs

        /// <summary>
        /// The result of the dialog.
        /// </summary>
        IDialogResult Result { get; set; }
    }
}
