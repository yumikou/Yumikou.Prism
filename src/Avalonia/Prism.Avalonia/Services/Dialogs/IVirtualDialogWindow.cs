
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
        object Content { get; set; }

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
        /// Called when the window is opened.
        /// </summary>
        event EventHandler<EventArgs> Opened;

        /// <summary>
        /// Called when the window is closed.
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Called when the window is closing.
        /// </summary>
        event EventHandler<VirtualWindowClosingEventArgs>? Closing;

        /// <summary>
        /// The result of the dialog.
        /// </summary>
        IDialogResult Result { get; set; }

        
        Styles Styles { get; }
    }
}
