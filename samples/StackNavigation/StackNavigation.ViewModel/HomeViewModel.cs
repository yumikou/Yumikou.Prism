using System;
using System.Collections.Generic;
using System.Text;
using Prism.Regions;
using System.Linq;
using Prism.Commands;

namespace StackNavigation.ViewModel
{
    public class HomeViewModel : RegionStackNavigationBase
    {
        private IRegionManager _regionManager;

        public int StackDepth { get; set; }

        public DelegateCommand GoNextPageCommand { get; set; }

        public HomeViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            StackDepth = _regionManager.Regions[RegionNames.MainRegion].Views.Count();
            GoNextPageCommand = new DelegateCommand(OnGoNextPage);
        }

        private void OnGoNextPage()
        {
            _regionManager.RequestStackNavigate(RegionNames.MainRegion, "SecondView");
        }
    }
}
