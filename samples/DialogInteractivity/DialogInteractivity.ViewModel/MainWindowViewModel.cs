using System;
using System.Collections.Generic;
using System.Text;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace DialogInteractivity.ViewModel
{
    public class MainWindowViewModel: BindableBase
    {
        public DelegateCommand UseDialogServiceControlCommand { get; set; }
        public DelegateCommand UseDialogServiceControlClosedCommand { get; set; }

        public DelegateCommand UseDialogServiceCommand { get; set; }

        public DelegateCommand UseDialogInteractionRequestCommand { get; set; }

        public MainWindowViewModel(IDialogService dialogService)
        {
            UseDialogServiceControlCommand = new DelegateCommand(OnUseDialogServiceControl);
            UseDialogServiceControlClosedCommand = new DelegateCommand(OnUseDialogServiceControlClosed);

            _dialogService = dialogService;
            UseDialogServiceCommand = new DelegateCommand(OnUseDialogService);

            UseDialogInteractionRequestCommand = new DelegateCommand(OnUseDialogInteractionRequest);
        }

        #region DialogServiceControl

        private DialogParameters _dialogParameters;
        public DialogParameters DialogParameters
        { 
            get { return _dialogParameters; }
            set { SetProperty<DialogParameters>(ref _dialogParameters, value); }
        }

        private IDialogResult _dialogResult;
        public IDialogResult DialogResult
        {
            get { return _dialogResult; }
            set { SetProperty<IDialogResult>(ref _dialogResult, value); }
        }

        private bool _isShow = false;
        public bool IsShow
        {
            get { return _isShow; }
            set
            {
                SetProperty<bool>(ref _isShow, value);
            }
        }

        private void OnUseDialogServiceControl()
        {
            DialogParameters = new DialogParameters();
            DialogParameters.Add("Title", "UseDialogServiceControl");
            IsShow = !IsShow;
        }

        private void OnUseDialogServiceControlClosed()
        {
            var r = DialogResult;
        }

        #endregion

        #region DialogService

        private IDialogService _dialogService;

        private void OnUseDialogService()
        {
            var paramerters = new DialogParameters();
            paramerters.Add("Title", "UseDialogService");
            _dialogService.ShowDialog("MyDialogView", paramerters, result =>
            {
                
            });
        }

        #endregion

        #region UseDialogInteractionRequest

        public DialogInteractionRequest DialogInteractionRequest { get; set; } = new DialogInteractionRequest();

        private void OnUseDialogInteractionRequest()
        {
            var paramerters = new DialogParameters();
            paramerters.Add("Title", "UseDialogInteractionRequest");
            DialogInteractionRequest.ShowDialog("MyDialogView", paramerters, result =>
            {
                
            });
        }

        #endregion
    }
}
