using Prism.Regions;
using Prism.Navigation;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Prism.Services.Dialogs;

namespace NestedRegion.ViewModel
{
    public class AViewModel : ViewModelHeaderBase, IRegionMemberLifetime, IDestructible, INavigationAware
    {
        private IRegionManager _regionManager;
        private IDialogService _dialogService;

        public DelegateCommand ShowDialogCommand { get; set; }

        public RegionMemberLifetimeType RegionMemberLifetimeType { get; } = RegionMemberLifetimeType.Transient;

        public AViewModel(IDialogService dialogService, IRegionManager regionManager)
        {
            _dialogService = dialogService;
            ShowDialogCommand = new DelegateCommand(OnShowDialog);
            _regionManager = regionManager;
        }

        private void OnShowDialog()
        {
            _dialogService.ShowDialog("DialogView");
        }

        public void Destroy()
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var dd = _regionManager;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
    }
}
