﻿using System;
using System.Collections.Generic;
using System.Text;
using Prism.Regions;
using System.Linq;
using Prism.Commands;
using System.Diagnostics;

namespace StackNavigation.ViewModel
{
    public class SecondViewModel : RegionStackNavigationBase
    {
        private IRegionManager _regionManager;

        public int StackDepth { get; set; }

        public DelegateCommand GoNextPageCommand { get; set; }

        public SecondViewModel(IRegionManager regionManager)
        {
            Debug.WriteLine("Create SecondPage=" + this.GetHashCode());
            _regionManager = regionManager;
            StackDepth = _regionManager.Regions[RegionNames.MainRegion].Views.Count();
            GoNextPageCommand = new DelegateCommand(OnGoNextPage);
        }

        private void OnGoNextPage()
        {
            _regionManager.RequestStackNavigate(RegionNames.MainRegion, "SecondView");
        }

        ~SecondViewModel()
        {
            Debug.WriteLine("Release SecondPage=" + this.GetHashCode());
        }
    }
}