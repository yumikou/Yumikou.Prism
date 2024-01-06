using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism.Commands;

namespace StackNavigation.ViewModel
{
    public class MainWindowViewModel
    {
        private IRegionManager _regionManager;
        private bool _isViewLoaded = false;

        public DelegateCommand ViewLoadedCommand { get; set; }
        public DelegateCommand GoHomeCommand { get; set; }
        public DelegateCommand GoBackCommand { get; set; }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            ViewLoadedCommand = new DelegateCommand(OnViewLoaded);
            GoHomeCommand = new DelegateCommand(OnGoHome);
            GoBackCommand = new DelegateCommand(OnGoBack);
        }

        private void OnViewLoaded()
        {
            if (!_isViewLoaded)
            {
                _isViewLoaded = true;
                if (_regionManager.Regions.ContainsRegionWithName(RegionNames.MainRegion))
                {
                    _regionManager.RequestStackNavigate(RegionNames.MainRegion, "HomeView");
                }
            }
        }

        private void OnGoHome()
        {
            _regionManager.Regions[RegionNames.MainRegion].NavigationService.Journal.GoBack("HomeView");
#if DEBUG
            System.GC.Collect();
#endif
        }

        private void OnGoBack()
        {
            _regionManager.Regions[RegionNames.MainRegion].NavigationService.Journal.GoBack();
#if DEBUG
            System.GC.Collect();
#endif
        }
    }
}
