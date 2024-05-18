using Prism.Regions;
using Prism.Navigation;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Prism.Services.Dialogs;

namespace NestedRegion.ViewModel
{
    public class AViewModel : ViewModelHeaderBase, IRegionMemberLifetime, IDestructible
    {
        private IDialogService _dialogService;

        public DelegateCommand ShowDialogCommand { get; set; }

        public RegionMemberLifetimeType RegionMemberLifetimeType { get; } = RegionMemberLifetimeType.Transient;

        public AViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            ShowDialogCommand = new DelegateCommand(OnShowDialog);
        }

        private void OnShowDialog()
        {
            _dialogService.ShowDialog("DialogView");
        }

        public void Destroy()
        {

        }
    }
}
