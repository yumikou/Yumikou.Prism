using Prism.Ioc;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Collections.ObjectModel;

namespace Prism.Services.Dialogs
{
    public class DialogHost : FrameworkElement
    {
        private readonly IContainerExtension _containerExtension;
        private IDialogWindow _dialogWindow;
        private bool _isShowAsyncChangedEnabled = true;

        public event EventHandler<DialogResultEventArgs> Closed;

        #region DependencyProperty

        /// <summary>
        /// DependencyProperty for <see cref="IsShowAsync" /> property.
        /// </summary>
        public static readonly DependencyProperty IsShowAsyncProperty =
            DependencyProperty.Register(
                "IsShowAsync",
                typeof(bool),
                typeof(DialogHost),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsShowAsyncPropertyChanged));

        private static void OnIsShowAsyncPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is DialogHost dsc && dsc._isShowAsyncChangedEnabled && e.NewValue is bool isShow)
            {
                if (isShow)
                {
                    dsc.Open(false);
                }
                else
                {
                    dsc.Close();
                }
            }
        }

        /// <summary>
        /// Determines if the content should be shown in a modal window or not.
        /// </summary>
        public static readonly DependencyProperty IsModalProperty =
            DependencyProperty.Register(
                "IsModal",
                typeof(bool),
                typeof(DialogHost),
                new PropertyMetadata(false));

        /// <summary>
        /// DependencyProperty for <see cref="DialogName" /> property.
        /// </summary>
        public static readonly DependencyProperty DialogNameProperty =
            DependencyProperty.Register(
                "DialogName",
                typeof(string),
                typeof(DialogHost),
                new PropertyMetadata(null));

        /// <summary>
        /// DependencyProperty for <see cref="WindowName" /> property.
        /// </summary>
        public static readonly DependencyProperty WindowNameProperty =
            DependencyProperty.Register(
                "WindowName",
                typeof(string),
                typeof(DialogHost),
                new PropertyMetadata(null));

        /// <summary>
        /// DependencyProperty for <see cref="WindowStyle" /> property.
        /// </summary>
        public static readonly DependencyProperty WindowStyleProperty =
            Dialog.WindowStyleProperty.AddOwner(typeof(DialogHost));

        /// <summary>
        /// DependencyProperty for <see cref="Parameters" /> property.
        /// </summary>
        public static readonly DependencyProperty ParametersProperty =
            DependencyProperty.Register(
                "Parameters",
                typeof(IDialogParameters),
                typeof(DialogHost),
                new PropertyMetadata(null));

        /// <summary>
        /// DependencyProperty for <see cref="Result" /> property.
        /// </summary>
        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register(
                "Result",
                typeof(IDialogResult),
                typeof(DialogHost),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// DependencyProperty for <see cref="Owner" /> property.
        /// </summary>
        public static readonly DependencyProperty OwnerProperty =
            DependencyProperty.Register(
                "Owner",
                typeof(Window),
                typeof(DialogHost),
                new PropertyMetadata(null));

        /// <summary>
        /// DependencyProperty for <see cref="IsOwnerEnabled" /> property.
        /// </summary>
        public static readonly DependencyProperty IsOwnerEnabledProperty =
            DependencyProperty.Register(
                "IsOwnerEnabled",
                typeof(bool),
                typeof(DialogHost),
                new PropertyMetadata(true));

        /// <summary>
        /// DependencyProperty for <see cref="DialogState" /> property.
        /// </summary>
        public static readonly DependencyProperty DialogStateProperty =
            DependencyProperty.Register(
                "DialogState",
                typeof(DialogState),
                typeof(DialogHost),
                new FrameworkPropertyMetadata(DialogState.Closed, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        #endregion

        /// <summary>
        /// Dialog is show.
        /// </summary>
        public bool IsShowAsync
        {
            get { return (bool)GetValue(IsShowAsyncProperty); }
            set { SetValue(IsShowAsyncProperty, value); }
        }

        /// <summary>
        /// Gets or sets if the window will be modal or not.
        /// </summary>
        public bool IsModal
        {
            get { return (bool)GetValue(IsModalProperty); }
            set { SetValue(IsModalProperty, value); }
        }

        public string DialogName
        {
            get { return (string)GetValue(DialogNameProperty); }
            set { SetValue(DialogNameProperty, value); }
        }

        public string WindowName
        {
            get { return (string)GetValue(WindowNameProperty); }
            set { SetValue(WindowNameProperty, value); }
        }

        public Style WindowStyle
        {
            get { return (Style)GetValue(WindowStyleProperty); }
            set { SetValue(WindowStyleProperty, value); }
        }

        public IDialogParameters Parameters
        {
            get { return (IDialogParameters)GetValue(ParametersProperty); }
            set { SetValue(ParametersProperty, value); }
        }

        public IDialogResult Result
        {
            get { return (IDialogResult)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
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

        public DialogState DialogState
        {
            get { return (DialogState)GetValue(DialogStateProperty); }
            protected set { SetValue(DialogStateProperty, value); }
        }

        public DialogHost()
        {
            this.Visibility = Visibility.Collapsed;
            _containerExtension = ContainerLocator.Current;
        }

        protected void SetCurrentIsShowAsync(bool isShow)
        {
            _isShowAsyncChangedEnabled = false;
            SetCurrentValue(IsShowAsyncProperty, isShow);
            _isShowAsyncChangedEnabled = true;
        }

        public virtual void Open(bool isBlockedModal)
        {
            if (DialogState != DialogState.Closed) { throw new InvalidOperationException("The dialog must be closed before opening."); }

            SetCurrentValue(ResultProperty, null);
            SetCurrentIsShowAsync(true);
            SetCurrentValue(DialogStateProperty, DialogState.Opening);
            _dialogWindow = CreateDialogWindow(WindowName);
            ConfigureDialogWindowEvents(_dialogWindow);
            ConfigureDialogWindowContent(DialogName, _dialogWindow, Parameters);
            
            ShowDialogWindow(_dialogWindow, IsModal, isBlockedModal);
        }

        public virtual void Close()
        {
            if (DialogState != DialogState.Opened) { throw new InvalidOperationException("The dialog has been closed."); }

            //this.Dispatcher.BeginInvoke(() =>
            //{
                _dialogWindow?.Close();
            //});
        }

        /// <summary>
        /// Shows the dialog window.
        /// </summary>
        /// <param name="dialogWindow">The dialog window to show.</param>
        /// <param name="isModal">If true; dialog is shown as a modal</param>
        protected virtual void ShowDialogWindow(IDialogWindow dialogWindow, bool isModal, bool isBlockedModal)
        {
            if (IsOwnerEnabled)
            {
                if (Owner != null)
                {
                    dialogWindow.Owner = Owner;
                }
                else if (dialogWindow.Owner == null)
                {
                    dialogWindow.Owner = Window.GetWindow(this); // DialogHost在xaml中构建时，所在的View未必已添加到控件树上，所以尽可能的延迟Window.GetWindow的访问时机
                }
            }

            if (isModal)
            {
                if (isBlockedModal)
                {
                    dialogWindow.ShowDialog();
                }
                else
                {
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        dialogWindow.ShowDialog();
                    });
                }
            }
            else
            {
                dialogWindow.Show();
            } 
        }

        /// <summary>
        /// Create a new <see cref="IDialogWindow"/>.
        /// </summary>
        /// <param name="name">The name of the hosting window registered with the IContainerRegistry.</param>
        /// <returns>The created <see cref="IDialogWindow"/>.</returns>
        protected virtual IDialogWindow CreateDialogWindow(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return _containerExtension.Resolve<IDialogWindow>();
            else
                return _containerExtension.Resolve<IDialogWindow>(name);
        }

        /// <summary>
        /// Configure <see cref="IDialogWindow"/> content.
        /// </summary>
        /// <param name="dialogName">The name of the dialog to show.</param>
        /// <param name="window">The hosting window.</param>
        /// <param name="parameters">The parameters to pass to the dialog.</param>
        protected virtual void ConfigureDialogWindowContent(string dialogName, IDialogWindow window, IDialogParameters parameters)
        {
            if (parameters == null)
            {
                parameters = new DialogParameters(); 
            }

            var content = _containerExtension.Resolve<object>(dialogName);
            if (!(content is FrameworkElement dialogContent))
                throw new NullReferenceException("A dialog's content must be a FrameworkElement");

            MvvmHelpers.AutowireViewModel(dialogContent);

            if (!(dialogContent.DataContext is IDialogAware viewModel))
                throw new NullReferenceException("A dialog's ViewModel must implement the IDialogAware interface");

            ConfigureDialogWindowProperties(window, dialogContent, viewModel);

            MvvmHelpers.ViewAndViewModelAction<IDialogAware>(viewModel, d => d.OnDialogOpened(parameters));
        }

        /// <summary>
        /// Configure <see cref="IDialogWindow"/> and <see cref="IDialogAware"/> events.
        /// </summary>
        /// <param name="dialogWindow">The hosting window.</param>
        /// <param name="callback">The action to perform when the dialog is closed.</param>
        protected virtual void ConfigureDialogWindowEvents(IDialogWindow dialogWindow)
        {
            Action<IDialogResult> requestCloseHandler = null;
            requestCloseHandler = (o) =>
            {
                dialogWindow.Result = o;
                Close();
            };

            RoutedEventHandler loadedHandler = null;
            loadedHandler = (o, e) =>
            {
                dialogWindow.Loaded -= loadedHandler;
                dialogWindow.GetDialogViewModel().RequestClose += requestCloseHandler;
                SetCurrentValue(DialogStateProperty, DialogState.Opened);
            };
            dialogWindow.Loaded += loadedHandler;

            CancelEventHandler closingHandler = null;
            closingHandler = (o, e) =>
            {
                var lastDialogState = DialogState;
                if (dialogWindow.Result == null)
                {
                    dialogWindow.Result = new DialogResult();
                }
                SetCurrentIsShowAsync(false);
                SetCurrentValue(DialogStateProperty, DialogState.Closing);
                if (!dialogWindow.GetDialogViewModel().CanCloseDialog(dialogWindow.Result))
                {
                    SetCurrentIsShowAsync(true);
                    SetCurrentValue(DialogStateProperty, lastDialogState);
                    e.Cancel = true;
                }
            };
            dialogWindow.Closing += closingHandler;

            EventHandler closedHandler = null;
            closedHandler = (o, e) =>
            {
                _dialogWindow = null;
                dialogWindow.Closed -= closedHandler;
                dialogWindow.Closing -= closingHandler;
                dialogWindow.GetDialogViewModel().RequestClose -= requestCloseHandler;
                dialogWindow.GetDialogViewModel().OnDialogClosed(dialogWindow.Result);

                SetCurrentValue(DialogStateProperty, DialogState.Closed);
                SetCurrentValue(ResultProperty, dialogWindow.Result);
                Closed?.Invoke(this, new DialogResultEventArgs(Result));

                dialogWindow.DataContext = null;
                dialogWindow.Content = null;
            };
            dialogWindow.Closed += closedHandler;
        }

        /// <summary>
        /// Configure <see cref="IDialogWindow"/> properties.
        /// </summary>
        /// <param name="window">The hosting window.</param>
        /// <param name="dialogContent">The dialog to show.</param>
        /// <param name="viewModel">The dialog's ViewModel.</param>
        protected virtual void ConfigureDialogWindowProperties(IDialogWindow window, FrameworkElement dialogContent, IDialogAware viewModel)
        {
            if (WindowStyle != null)
            {
                window.Style = WindowStyle;
            }
            else
            {
                var windowStyle = Dialog.GetWindowStyle(dialogContent);
                if (windowStyle != null)
                    window.Style = windowStyle;
            }

            window.Content = dialogContent;
            window.DataContext = viewModel; //we want the host window and the dialog to share the same data context
        }
    }
}
