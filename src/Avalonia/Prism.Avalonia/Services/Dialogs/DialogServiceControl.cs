using Prism.Ioc;
using Prism.Common;
using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Interactivity;
using Avalonia.Styling;
using DynamicData;
using Avalonia.Markup.Xaml.XamlIl.Runtime;
using Avalonia.Markup.Xaml;

namespace Prism.Services.Dialogs
{
    /// <summary>
    /// TODO: 与wpf的实现放入共享项目，IDialogWindow不做任何修改，完全通过宏命令差异化处理
    /// VirtualDialogServiceControl同理也和wpf的实现共享，但不和当前这个DialogServiceControl共享，而是都用新的逻辑
    /// </summary>
    public class DialogServiceControl : Control
    {
        private readonly IContainerExtension _containerExtension;
        private IDialogWindow _dialogWindow;
        private bool _isShowAsyncChangedEnabled = true;
        private DispatcherFrame _modalDialogDispatcherFrame;

        public event EventHandler<DialogResultEventArgs> Closed;

        #region DependencyProperty

        /// <summary>
        /// DependencyProperty for <see cref="IsShowAsync" /> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsShowAsyncProperty =
            AvaloniaProperty.Register<DialogServiceControl, bool>("IsShowAsync", false, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        private static void OnIsShowAsyncPropertyChanged(AvaloniaObject sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (sender is DialogServiceControl dsc && dsc._isShowAsyncChangedEnabled && e.NewValue is bool isShow)
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
        /// DependencyProperty for <see cref="IsModal" /> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsModalProperty =
            AvaloniaProperty.Register<DialogServiceControl, bool>("IsModal", false);

        /// <summary>
        /// DependencyProperty for <see cref="DialogName" /> property.
        /// </summary>
        public static readonly StyledProperty<string> DialogNameProperty =
            AvaloniaProperty.Register<DialogServiceControl, string>("DialogName", null);

        /// <summary>
        /// DependencyProperty for <see cref="WindowName" /> property.
        /// </summary>
        public static readonly StyledProperty<string> WindowNameProperty =
            AvaloniaProperty.Register<DialogServiceControl, string>("WindowName", null);

        /// <summary>
        /// DependencyProperty for <see cref="WindowStyle" /> property.
        /// </summary>
        public static readonly StyledProperty<Style> WindowStyleProperty =
            Dialog.WindowStyleProperty.AddOwner<DialogServiceControl>();

        /// <summary>
        /// DependencyProperty for <see cref="Parameters" /> property.
        /// </summary>
        public static readonly StyledProperty<IDialogParameters> ParametersProperty =
            AvaloniaProperty.Register<DialogServiceControl, IDialogParameters>("Parameters", null);

        /// <summary>
        /// DependencyProperty for <see cref="Result" /> property.
        /// </summary>
        public static readonly StyledProperty<IDialogResult> ResultProperty =
            AvaloniaProperty.Register<DialogServiceControl, IDialogResult>("Result", null, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        /// <summary>
        /// DependencyProperty for <see cref="Owner" /> property.
        /// </summary>
        public static readonly StyledProperty<Window> OwnerProperty =
            AvaloniaProperty.Register<DialogServiceControl, Window>("Owner", null);

        /// <summary>
        /// DependencyProperty for <see cref="IsOwnerEnabled" /> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsOwnerEnabledProperty =
            AvaloniaProperty.Register<DialogServiceControl, bool>("IsOwnerEnabled", true);

        /// <summary>
        /// DependencyProperty for <see cref="DialogState" /> property.
        /// </summary>
        public static readonly StyledProperty<DialogState> DialogStateProperty =
            AvaloniaProperty.Register<DialogServiceControl, DialogState>("DialogState", DialogState.Closed, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        #endregion

        /// <summary>
        /// Dialog is show.
        /// </summary>
        public bool IsShowAsync
        {
            get { return (bool)GetValue(IsShowAsyncProperty); }
            set { 
                SetValue(IsShowAsyncProperty, value);
            }
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
            get { return GetValue(WindowStyleProperty); }
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

        static DialogServiceControl()
        {
            IsShowAsyncProperty.Changed.Subscribe(args => OnIsShowAsyncPropertyChanged(args?.Sender, args));
        }

        public DialogServiceControl()
        {
            this.IsVisible = false;
            
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
            if (DialogState != DialogState.Loaded) { throw new InvalidOperationException("The dialog has been closed."); }

            _dialogWindow?.Close();
        }

        /// <summary>
        /// Shows the dialog window.
        /// </summary>
        /// <param name="dialogWindow">The dialog window to show.</param>
        /// <param name="isModal">If true; dialog is shown as a modal</param>
        protected virtual void ShowDialogWindow(IDialogWindow dialogWindow, bool isModal, bool isBlockedModal)
        {
            Window ownerWindow = null;
            if (IsOwnerEnabled)
            {
                if (Owner != null)
                {
                    ownerWindow = Owner;
                }
                else if (dialogWindow.Owner == null)
                {
                    ownerWindow = Window.GetTopLevel(this) as Window;
                }
            }

            if (isModal)
            {
                if (isBlockedModal)
                {
                    _modalDialogDispatcherFrame = new DispatcherFrame();
                    dialogWindow.ShowDialog(ownerWindow);
                    Dispatcher.UIThread.PushFrame(_modalDialogDispatcherFrame);
                }
                else
                {
                    dialogWindow.ShowDialog(ownerWindow);
                }
            }
            else
            {
                if (ownerWindow != null)
                {
                    dialogWindow.Show(ownerWindow);
                }
                else
                {
                    dialogWindow.Show();
                }
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
            if (!(content is Avalonia.Controls.Control dialogContent))
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

            EventHandler<RoutedEventArgs> loadedHandler = null;
            loadedHandler = (o, e) =>
            {
                dialogWindow.Loaded -= loadedHandler;
                dialogWindow.GetDialogViewModel().RequestClose += requestCloseHandler;
                SetCurrentValue(DialogStateProperty, DialogState.Loaded);
            };
            dialogWindow.Loaded += loadedHandler;

            EventHandler<WindowClosingEventArgs> closingHandler = null;
            closingHandler = (o, e) =>
            {
                var lastDialogState = DialogState;
                SetCurrentIsShowAsync(false);
                SetCurrentValue(DialogStateProperty, DialogState.Closing);
                if (!dialogWindow.GetDialogViewModel().CanCloseDialog())
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
                dialogWindow.GetDialogViewModel().OnDialogClosed();

                if (dialogWindow.Result == null)
                {
                    dialogWindow.Result = new DialogResult();
                }
                SetCurrentValue(DialogStateProperty, DialogState.Closed);
                SetCurrentValue(ResultProperty, dialogWindow.Result);
                Closed?.Invoke(this, new DialogResultEventArgs(Result));

                dialogWindow.DataContext = null;
                dialogWindow.Content = null;
                if (_modalDialogDispatcherFrame != null)
                {
                    _modalDialogDispatcherFrame.Continue = false;
                    _modalDialogDispatcherFrame = null;
                }
            };
            dialogWindow.Closed += closedHandler;
        }

        /// <summary>
        /// Configure <see cref="IDialogWindow"/> properties.
        /// </summary>
        /// <param name="window">The hosting window.</param>
        /// <param name="dialogContent">The dialog to show.</param>
        /// <param name="viewModel">The dialog's ViewModel.</param>
        protected virtual void ConfigureDialogWindowProperties(IDialogWindow window, Avalonia.Controls.Control dialogContent, IDialogAware viewModel)
        {
            var dialogWindowStyle = Dialog.GetWindowStyle(dialogContent);
            if (dialogWindowStyle != null)
            {
                window.Styles?.Add(dialogWindowStyle);
            }
            if (WindowStyle != null)
            {
                window.Styles?.Add(WindowStyleClone(WindowStyle));
            }

            window.Content = dialogContent;
            window.DataContext = viewModel; //we want the host window and the dialog to share the same data context
        }

        protected Style WindowStyleClone(Style source)
        {
            if (source == null) { return null; }
            
            Style resultStyle = new Style();
            resultStyle.Selector = source.Selector;

            foreach (var ani in source.Animations)
            {
                resultStyle.Animations.Add(ani);
            }
            foreach (var chi in source.Children)
            {
                resultStyle.Children.Add(chi);
            }
            foreach (var res in source.Resources)
            {
                resultStyle.Resources.Add(res);
            }
            foreach (var set in source.Setters)
            {
                resultStyle.Setters.Add(set);
            }
            
            return resultStyle;
        }
    }
}
