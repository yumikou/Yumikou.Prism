using System;
using System.Collections.Generic;
using System.Text;
using Prism.Regions;
using Prism.Commands;
using Prism.Mvvm;

namespace NestedRegion.ViewModel
{
    public class MainWindowViewModel:BindableBase
    {
        private IRegionManager _regionManager;
        private bool _isViewLoaded = false;

        public DelegateCommand ViewLoadedCommand { get; set; }

        public List<string> TabHeaders { get; set; } = new List<string>() { "A", "B" };

        private string _tabSelectedItem;
        public string TabSelectedItem
        {
            get { return _tabSelectedItem; }
            set
            {
                if (SetProperty(ref _tabSelectedItem, value))
                {
                    if (_regionManager.Regions.ContainsRegionWithName(RegionNames.MainRegion))
                    {
                        _regionManager.RequestNavigate(RegionNames.MainRegion, $"{value}View");
                    }  
                }
            }
        }

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
                    _regionManager.RequestNavigate(RegionNames.MainRegion, $"{TabHeaders[0]}View");
                }
            }
        }
    }
}
