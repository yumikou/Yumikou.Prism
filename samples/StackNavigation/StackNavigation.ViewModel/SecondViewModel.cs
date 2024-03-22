using System;
using System.Collections.Generic;
using System.Text;
using Prism.Regions;
using System.Linq;
using Prism.Commands;
using System.Diagnostics;

namespace StackNavigation.ViewModel
{
    public class SecondViewModel : INavigationAware, IRegionMemberLifetime
    {
        private IRegionManager _regionManager;

        public int StackDepth { get; set; }

        public DelegateCommand GoNextPageCommand { get; set; }

        public RegionMemberLifetimeType RegionMemberLifetimeType { get; set; } = RegionMemberLifetimeType.Stack;

        public SecondViewModel(IRegionManager regionManager)
        {
            Debug.WriteLine("Create SecondPage=" + this.GetHashCode());
            _regionManager = regionManager;
            StackDepth = _regionManager.Regions[RegionNames.MainRegion].Views.Count();
            GoNextPageCommand = new DelegateCommand(OnGoNextPage);
        }

        private void OnGoNextPage()
        {
            _regionManager.RequestNavigate(RegionNames.MainRegion, "SecondView");
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        ~SecondViewModel()
        {
            Debug.WriteLine("Release SecondPage=" + this.GetHashCode());
        }
    }
}
