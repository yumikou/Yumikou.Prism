using Prism.Ioc;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace Prism.Services.Dialogs
{
    public class DialogServiceControl : DependencyObject
    {
        private readonly IContainerExtension _containerExtension;
        protected IDialogWindow _dialogWindow;
        private bool _isShowChangedEnable = true;
        private static readonly object _unsetOwner = new object();

        public event EventHandler<DialogResultEventArgs> Closed;

        #region DependencyProperty

        /// <summary>
        /// DependencyProperty for <see cref="IsShow" /> property.
        /// </summary>
        public static readonly DependencyProperty IsShowProperty =
            DependencyProperty.Register(
                "IsShow",
                typeof(bool),
                typeof(DialogServiceControl),
                new PropertyMetadata(false, OnIsShowPropertyChanged));

        private static void OnIsShowPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is DialogServiceControl dsc && dsc._isShowChangedEnable && e.NewValue is bool isShow)
            {
                if (isShow)
                {
                    dsc.Open();
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
                typeof(DialogServiceControl),
                new PropertyMetadata(false));

        /// <summary>
        /// DependencyProperty for <see cref="DialogName" /> property.
        /// </summary>
        public static readonly DependencyProperty DialogNameProperty =
            DependencyProperty.Register(
                "DialogName",
                typeof(string),
                typeof(DialogServiceControl),
                new PropertyMetadata(null));

        /// <summary>
        /// DependencyProperty for <see cref="WindowName" /> property.
        /// </summary>
        public static readonly DependencyProperty WindowNameProperty =
            DependencyProperty.Register(
                "WindowName",
                typeof(string),
                typeof(DialogServiceControl),
                new PropertyMetadata(null));

        /// <summary>
        /// DependencyProperty for <see cref="WindowStyle" /> property.
        /// </summary>
        public static readonly DependencyProperty WindowStyleProperty =
            Dialog.WindowStyleProperty.AddOwner(typeof(DialogServiceControl));

        /// <summary>
        /// DependencyProperty for <see cref="Parameters" /> property.
        /// </summary>
        public static readonly DependencyProperty ParametersProperty =
            DependencyProperty.Register(
                "Parameters",
                typeof(IDialogParameters),
                typeof(DialogServiceControl),
                new PropertyMetadata(null));

        /// <summary>
        /// DependencyProperty for <see cref="Result" /> property.
        /// </summary>
        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register(
                "Result",
                typeof(IDialogResult),
                typeof(DialogServiceControl),
                new PropertyMetadata(null));

        /// <summary>
        /// DependencyProperty for <see cref="Owner" /> property.
        /// </summary>
        public static readonly DependencyProperty OwnerProperty =
            DependencyProperty.Register(
                "Owner",
                typeof(Window),
                typeof(DialogServiceControl),
                new PropertyMetadata(_unsetOwner));

        /// <summary>
        /// DependencyProperty for <see cref="DialogState" /> property.
        /// </summary>
        public static readonly DependencyProperty DialogStateProperty =
            DependencyProperty.Register(
                "DialogState",
                typeof(DialogState),
                typeof(DialogServiceControl),
                new PropertyMetadata(DialogState.Closed));

        #endregion

        /// <summary>
        /// Dialog is show.
        /// </summary>
        public bool IsShow
        {
            get { return (bool)GetValue(IsShowProperty); }
            set { SetValue(IsShowProperty, value); }
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

        public DialogState DialogState
        {
            get { return (DialogState)GetValue(DialogStateProperty); }
            protected set { SetValue(DialogStateProperty, value); }
        }

        public DialogServiceControl()
        {
            _containerExtension = ContainerLocator.Current;
        }

        protected void SetCurrentIsShow(bool isShow)
        {
            _isShowChangedEnable = false;
            SetCurrentValue(IsShowProperty, isShow);
            _isShowChangedEnable = true;
        }

        protected virtual void Open()
        {
            if (DialogState != DialogState.Closed) { throw new InvalidOperationException("The dialog must be closed before opening."); }

            SetCurrentValue(DialogStateProperty, DialogState.Opening);
            _dialogWindow = CreateDialogWindow(WindowName);
            ConfigureDialogWindowEvents(_dialogWindow);
            ConfigureDialogWindowContent(DialogName, _dialogWindow, Parameters);
            ShowDialogWindow(_dialogWindow, IsModal);
        }

        protected virtual void Close()
        {
            if (DialogState != DialogState.Opened) { throw new InvalidOperationException("The dialog has been closed."); }

            _dialogWindow.Close();
        }

        /// <summary>
        /// Shows the dialog window.
        /// </summary>
        /// <param name="dialogWindow">The dialog window to show.</param>
        /// <param name="isModal">If true; dialog is shown as a modal</param>
        protected virtual void ShowDialogWindow(IDialogWindow dialogWindow, bool isModal)
        {
            if (isModal)
                dialogWindow.ShowDialog();
            else
                dialogWindow.Show();
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
                SetCurrentValue(ResultProperty, o);
                Close();
            };

            RoutedEventHandler loadedHandler = null;
            loadedHandler = (o, e) =>
            {
                dialogWindow.Loaded -= loadedHandler;
                dialogWindow.GetDialogViewModel().RequestClose += requestCloseHandler;
                SetCurrentIsShow(true);
                SetCurrentValue(DialogStateProperty, DialogState.Opened);
            };
            dialogWindow.Loaded += loadedHandler;

            CancelEventHandler closingHandler = null;
            closingHandler = (o, e) =>
            {
                SetCurrentValue(DialogStateProperty, DialogState.Closing);
                if (!dialogWindow.GetDialogViewModel().CanCloseDialog())
                {
                    SetCurrentIsShow(true);
                    SetCurrentValue(DialogStateProperty, DialogState.Opened);
                    e.Cancel = true;
                }    
            };
            dialogWindow.Closing += closingHandler;

            EventHandler closedHandler = null;
            closedHandler = (o, e) =>
            {
                dialogWindow.Closed -= closedHandler;
                dialogWindow.Closing -= closingHandler;
                dialogWindow.GetDialogViewModel().RequestClose -= requestCloseHandler;

                dialogWindow.GetDialogViewModel().OnDialogClosed();

                dialogWindow.DataContext = null;
                dialogWindow.Content = null;
                _dialogWindow = null;

                //优先级 Result > dialogWindow.Result
                if (Result == null)
                {
                    SetCurrentValue(ResultProperty, dialogWindow.Result ?? new DialogResult());
                }
                SetCurrentIsShow(false);
                SetCurrentValue(DialogStateProperty, DialogState.Closed);
                Closed?.Invoke(this, new DialogResultEventArgs(Result));
                SetCurrentValue(ResultProperty, null);
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

            if (Owner != _unsetOwner)
            {
                window.Owner = Owner;
            }
            else if (window.Owner == null)
            {
                window.Owner = Window.GetWindow(this);
            }

            // window.Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
        }
    }
}
