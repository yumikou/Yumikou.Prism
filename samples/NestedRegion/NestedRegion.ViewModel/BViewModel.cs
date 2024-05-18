using System;
using System.Collections.Generic;
using System.Text;
using Prism.Regions;
using Prism.Navigation;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace NestedRegion.ViewModel
{
    public class BViewModel : ViewModelHeaderBase, IRegionMemberLifetime, IDestructible
    {
        private IRegionManager _regionManager;
        private bool _isViewLoaded = false;

        public RegionMemberLifetimeType RegionMemberLifetimeType { get; } = RegionMemberLifetimeType.Transient;

        public DelegateCommand ViewLoadedCommand { get; set; }

        public BViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            ViewLoadedCommand = new DelegateCommand(OnViewLoaded);
        }

        private void OnViewLoaded()
        {
            if (!_isViewLoaded)
            {
                _isViewLoaded = true;
                if (_regionManager.Regions.ContainsRegionWithName(RegionNames.NestedBRegion))
                {
                    _regionManager.RequestCreate(RegionNames.NestedBRegion, "BN1View?Title=BN1");
                    _regionManager.RequestCreate(RegionNames.NestedBRegion, "BN2View?Title=BN2");
                    _regionManager.RequestCreate(RegionNames.NestedBRegion, "BN3View?Title=BN3");
                }
            }
        }

        public void Destroy()
        {

        }
    }
}
