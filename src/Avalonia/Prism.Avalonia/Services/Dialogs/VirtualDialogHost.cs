﻿using Prism.Ioc;
using Prism.Common;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
using Avalonia.Styling;
using Avalonia;
using System.Threading;
using Prism.Extensions;
using Avalonia.Controls.Primitives;

namespace Prism.Services.Dialogs
{
    public class VirtualDialogHost: Control
    {
        private readonly IContainerExtension _containerExtension;
        private IVirtualDialogWindow _dialogWindow;
        private bool _isShowAsyncChangedEnabled = true;

        public event EventHandler<DialogResultEventArgs> Closed;


        #region DependencyProperty

        /// <summary>
        /// DependencyProperty for <see cref="IsShowAsync" /> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsShowAsyncProperty =
            AvaloniaProperty.Register<VirtualDialogHost, bool>("IsShowAsync", false, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        private static void OnIsShowAsyncPropertyChanged(AvaloniaObject sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (sender is VirtualDialogHost dsc && dsc._isShowAsyncChangedEnabled && e.NewValue is bool isShow)
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
        /// DependencyProperty for <see cref="DialogName" /> property.
        /// </summary>
        public static readonly StyledProperty<string> DialogNameProperty =
            AvaloniaProperty.Register<VirtualDialogHost, string>("DialogName", null);

        /// <summary>
        /// DependencyProperty for <see cref="WindowName" /> property.
        /// </summary>
        public static readonly StyledProperty<string> WindowNameProperty =
            AvaloniaProperty.Register<VirtualDialogHost, string>("WindowName", null);

        /// <summary>
        /// DependencyProperty for <see cref="VirtualWindowStyle" /> property.
        /// </summary>
        public static readonly StyledProperty<Style> VirtualWindowStyleProperty =
            Dialog.VirtualWindowStyleProperty.AddOwner<VirtualDialogHost>();

        /// <summary>
        /// DependencyProperty for <see cref="Parameters" /> property.
        /// </summary>
        public static readonly StyledProperty<IDialogParameters> ParametersProperty =
            AvaloniaProperty.Register<VirtualDialogHost, IDialogParameters>("Parameters", null);

        /// <summary>
        /// DependencyProperty for <see cref="Result" /> property.
        /// </summary>
        public static readonly StyledProperty<IDialogResult> ResultProperty =
            AvaloniaProperty.Register<VirtualDialogHost, IDialogResult>("Result", null, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        /// <summary>
        /// DependencyProperty for <see cref="Owner" /> property.
        /// </summary>
        public static readonly StyledProperty<Visual> OwnerProperty =
            AvaloniaProperty.Register<VirtualDialogHost, Visual>("Owner", null);

        /// <summary>
        /// DependencyProperty for <see cref="IsOwnerEnabled" /> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsOwnerEnabledProperty =
            AvaloniaProperty.Register<VirtualDialogHost, bool>("IsOwnerEnabled", true);

        /// <summary>
        /// DependencyProperty for <see cref="DialogState" /> property.
        /// </summary>
        public static readonly StyledProperty<DialogState> DialogStateProperty =
            AvaloniaProperty.Register<VirtualDialogHost, DialogState>("DialogState", DialogState.Closed, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        #endregion

        /// <summary>
        /// Dialog is show.
        /// </summary>
        public bool IsShowAsync
        {
            get { return (bool)GetValue(IsShowAsyncProperty); }
            set
            {
                SetValue(IsShowAsyncProperty, value);
            }
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

        public Style VirtualWindowStyle
        {
            get { return GetValue(VirtualWindowStyleProperty); }
            set { SetValue(VirtualWindowStyleProperty, value); }
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

        public DialogState DialogState
        {
            get { return (DialogState)GetValue(DialogStateProperty); }
            protected set { SetValue(DialogStateProperty, value); }
        }

        static VirtualDialogHost()
        {
            IsShowAsyncProperty.Changed.Subscribe(args => OnIsShowAsyncPropertyChanged(args?.Sender, args));
        }

        public VirtualDialogHost()
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

        public virtual void Open()
        {
            if (DialogState != DialogState.Closed) { throw new InvalidOperationException("The dialog must be closed before opening."); }

            SetCurrentValue(ResultProperty, null);
            SetCurrentIsShowAsync(true);
            SetCurrentValue(DialogStateProperty, DialogState.Opening);
            _dialogWindow = CreateDialogWindow(WindowName);
            ConfigureDialogWindowEvents(_dialogWindow);
            ConfigureDialogWindowContent(DialogName, _dialogWindow, Parameters);

            ShowDialogWindow(_dialogWindow);
        }

        public virtual void Close()
        {
            if (DialogState != DialogState.Opened) { throw new InvalidOperationException("The dialog has been closed."); }

            _dialogWindow?.Close();
        }

        /// <summary>
        /// Shows the dialog window.
        /// </summary>
        /// <param name="dialogWindow">The dialog window to show.</param>
        protected virtual void ShowDialogWindow(IVirtualDialogWindow dialogWindow)
        {
            if (IsOwnerEnabled)
            {
                if (Owner != null)
                {
                    dialogWindow.Owner = Owner;
                }
                else if (dialogWindow.Owner == null)
                {
                    dialogWindow.Owner = TopLevel.GetTopLevel(this); // DialogHost在xaml中构建时，所在的View未必已添加到控件树上，所以尽可能的延迟Window.GetWindow的访问时机
                }
            }

            dialogWindow.Open();
        }

        /// <summary>
        /// Create a new <see cref="IVirtualDialogWindow"/>.
        /// </summary>
        /// <param name="name">The name of the hosting window registered with the IContainerRegistry.</param>
        /// <returns>The created <see cref="IVirtualDialogWindow"/>.</returns>
        protected virtual IVirtualDialogWindow CreateDialogWindow(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return _containerExtension.Resolve<IVirtualDialogWindow>();
            else
                return _containerExtension.Resolve<IVirtualDialogWindow>(name);
        }

        /// <summary>
        /// Configure <see cref="IVirtualDialogWindow"/> content.
        /// </summary>
        /// <param name="dialogName">The name of the dialog to show.</param>
        /// <param name="window">The hosting window.</param>
        /// <param name="parameters">The parameters to pass to the dialog.</param>
        protected virtual void ConfigureDialogWindowContent(string dialogName, IVirtualDialogWindow window, IDialogParameters parameters)
        {
            // TODO: 和常规窗体不同，虚拟对话框可以是界面上的任意浮层，不一定要视为标准对话框，可以尝试新增DialogContent和DialogContentTemplate属性，如果设置了值就在这里构建一个ContentControl作为DialogContent，并覆盖掉DialogName。
            // 同时新增一个DialogDataContext属性，一旦绑定或设置值就覆盖掉DialogName对应的DataContext，也能作为刚刚构建的ContentControl的DataContext，注意这个DataContext并不会每次打开对话框就构建一个新的，而是一直存在的。它不一定要实现IDialogAware，也不是必须设置的，而是作为一个类似ComboBox的数据源，可以便利的为ContentControl提供数据源，不必刻意处理对话框的相关逻辑
            // 也许上面说的并不合理，特殊的浮层直接用Popup相关的控件会更好
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
        /// Configure <see cref="IVirtualDialogWindow"/> and <see cref="IDialogAware"/> events.
        /// </summary>
        /// <param name="dialogWindow">The hosting window.</param>
        /// <param name="callback">The action to perform when the dialog is closed.</param>
        protected virtual void ConfigureDialogWindowEvents(IVirtualDialogWindow dialogWindow)
        {
            Action<IDialogResult> requestCloseHandler = null;
            requestCloseHandler = (o) =>
            {
                dialogWindow.Result = o;
                Close();
            };

            EventHandler<EventArgs> openedHandler = null;
            openedHandler = (o, e) =>
            {
                dialogWindow.Opened -= openedHandler;
                dialogWindow.GetDialogViewModel().RequestClose += requestCloseHandler;
                SetCurrentValue(DialogStateProperty, DialogState.Opened);
            };
            dialogWindow.Opened += openedHandler;

            EventHandler<VirtualWindowClosingEventArgs> closingHandler = null;
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
        /// Configure <see cref="IVirtualDialogWindow"/> properties.
        /// </summary>
        /// <param name="window">The hosting window.</param>
        /// <param name="dialogContent">The dialog to show.</param>
        /// <param name="viewModel">The dialog's ViewModel.</param>
        protected virtual void ConfigureDialogWindowProperties(IVirtualDialogWindow window, Avalonia.Controls.Control dialogContent, IDialogAware viewModel)
        {
            var dialogWindowStyle = Dialog.GetVirtualWindowStyle(dialogContent);
            if (dialogWindowStyle != null)
            {
                window.Styles?.Add(dialogWindowStyle.SimpleClone());
            }
            if (VirtualWindowStyle != null)
            {
                window.Styles?.Add(VirtualWindowStyle.SimpleClone());
            }

            window.Content = dialogContent;
            window.DataContext = viewModel; //we want the host window and the dialog to share the same data context
        }
    }
}
