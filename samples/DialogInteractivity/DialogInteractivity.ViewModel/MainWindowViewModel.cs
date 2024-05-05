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
        public MainWindowViewModel(IDialogService dialogService)
        {
            UseDialogHostCommand = new DelegateCommand(OnUseDialogHost);
            UseDialogHostClosedCommand = new DelegateCommand(OnUseDialogHostClosed);

            _dialogService = dialogService;
            UseDialogServiceCommand = new DelegateCommand(OnUseDialogService);

            UseDialogInteractionRequestCommand = new DelegateCommand(OnUseDialogInteractionRequest);
        }

        #region DialogHost

        public DelegateCommand UseDialogHostCommand { get; set; }
        public DelegateCommand UseDialogHostClosedCommand { get; set; }

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

        private void OnUseDialogHost()
        {
            DialogParameters = new DialogParameters();
            DialogParameters.Add("Title", "UseDialogHost");
            IsShow = !IsShow;
        }

        private void OnUseDialogHostClosed()
        {
            var r = DialogResult;
        }

        #endregion

        #region DialogService

        public DelegateCommand UseDialogServiceCommand { get; set; }

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

        public DelegateCommand UseDialogInteractionRequestCommand { get; set; }

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
