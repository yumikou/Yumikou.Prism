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
        public DelegateCommand UseVirtualDialogServiceCommand { get; set; }

        public MainViewModel(IVirtualDialogService virtualDialogService)
        {
            _virtualDialogService = virtualDialogService;
            UseVirtualDialogServiceCommand = new DelegateCommand(OnUseVirtualDialogService);
        }

        #region DialogService

        private IVirtualDialogService _virtualDialogService;

        private void OnUseVirtualDialogService()
        {
            var paramerters = new DialogParameters();
            paramerters.Add("Title", "UseVirtualDialogService");
            _virtualDialogService.ShowDialogAsync("MyDialogView", paramerters);
        }

        #endregion
    }
}
