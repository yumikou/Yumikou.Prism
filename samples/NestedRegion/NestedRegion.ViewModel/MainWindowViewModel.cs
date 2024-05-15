using System;
using System.Collections.Generic;
using System.Text;
using Prism.Regions;
using Prism.Commands;

namespace NestedRegion.ViewModel
{
    public class MainWindowViewModel
    {
        private IRegionManager _regionManager;
        private bool _isViewLoaded = false;

        public DelegateCommand ViewLoadedCommand { get; set; }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            ViewLoadedCommand = new DelegateCommand(OnViewLoaded);
        }

        private void OnViewLoaded()
        {
            if (!_isViewLoaded)
            {
                _isViewLoaded = true;
                if (_regionManager.Regions.ContainsRegionWithName(RegionNames.MainRegion))
                {
                    _regionManager.RequestCreate(RegionNames.MainRegion, "AView");
                    _regionManager.RequestCreate(RegionNames.MainRegion, "BView");
                }
            }
        }
    }
}
