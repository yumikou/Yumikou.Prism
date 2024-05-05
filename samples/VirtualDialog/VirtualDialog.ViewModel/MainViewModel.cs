using System;
using System.Collections.Generic;
using System.Text;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace VirtualDialog.ViewModel
{
    public class MainViewModel : BindableBase
    {
        public MainViewModel(IVirtualDialogService virtualDialogService)
        {
            UseVirtualDialogHostCommand = new DelegateCommand(OnUseVirtualDialogHost);
            VirtualDialogHostClosedCommand = new DelegateCommand(OnVirtualDialogHostClosed);

            _virtualDialogService = virtualDialogService;
            UseVirtualDialogServiceCommand = new DelegateCommand(OnUseVirtualDialogService);

            UseVirtualDialogInteractionRequestCommand = new DelegateCommand(OnUseVirtualDialogInteractionRequest);
        }

        #region VirtualDialogHost

        public DelegateCommand UseVirtualDialogHostCommand { get; set; }
        public DelegateCommand VirtualDialogHostClosedCommand { get; set; }

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

        private void OnUseVirtualDialogHost()
        {
            DialogParameters = new DialogParameters();
            DialogParameters.Add("Title", "UseVirtualDialogHost, click the background of the dialog to close it");
            IsShow = !IsShow;
        }

        private void OnVirtualDialogHostClosed()
        {
            var r = DialogResult;
        }

        #endregion


        #region VirtualDialogService

        public DelegateCommand UseVirtualDialogServiceCommand { get; set; }

        private IVirtualDialogService _virtualDialogService;

        private void OnUseVirtualDialogService()
        {
            var paramerters = new DialogParameters();
            paramerters.Add("Title", "UseVirtualDialogService, click the background of the dialog to close it");
            _virtualDialogService.ShowDialogAsync("MyDialogView", paramerters);
        }

        #endregion

        #region VirtualDialogInteractionRequest

        public DelegateCommand UseVirtualDialogInteractionRequestCommand { get; set; }

        public VirtualDialogInteractionRequest VirtualDialogInteractionRequest { get; set; } = new VirtualDialogInteractionRequest();

        private void OnUseVirtualDialogInteractionRequest()
        {
            var paramerters = new DialogParameters();
            paramerters.Add("Title", "UseVirtualDialogInteractionRequest, click the background of the dialog to close it");
            VirtualDialogInteractionRequest.ShowDialogAsync("MyDialogView", paramerters);
        }

        #endregion
    }
}
