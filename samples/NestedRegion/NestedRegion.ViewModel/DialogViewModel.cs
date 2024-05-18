using System;
using System.Collections.Generic;
using System.Text;
using Prism.Regions;
using Prism.Navigation;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace NestedRegion.ViewModel
{
    public class DialogViewModel: ViewModelHeaderBase, IRegionMemberLifetime, IDestructible, IDialogAware
    {
        private IRegionManager _regionManager;
        private bool _isViewLoaded = false;

        public event Action<IDialogResult> RequestClose;

        public IRegionManager RegionManager
        {
            get { return _regionManager; }
            set
            {
                SetProperty(ref _regionManager, value);
            }
        }

        public RegionMemberLifetimeType RegionMemberLifetimeType { get; } = RegionMemberLifetimeType.Transient;

        public DelegateCommand ViewLoadedCommand { get; set; }

        public DialogViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager.CreateRegionManager();
            ViewLoadedCommand = new DelegateCommand(OnViewLoaded);
        }

        private void OnViewLoaded()
        {
            if (!_isViewLoaded)
            {
                _isViewLoaded = true;
                if (_regionManager.Regions.ContainsRegionWithName(RegionNames.DialogMainRegion))
                {
                    _regionManager.RequestCreate(RegionNames.DialogMainRegion, "BN1View?Title=BN1");
                    _regionManager.RequestCreate(RegionNames.DialogMainRegion, "BN2View?Title=BN2");
                    _regionManager.RequestCreate(RegionNames.DialogMainRegion, "BN3View?Title=BN3");
                }
            }
        }

        public void Destroy()
        {

        }

        public bool CanCloseDialog(IDialogResult dialogResult)
        {
            return true;
        }

        public void OnDialogClosed(IDialogResult dialogResult)
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }
    }
}
