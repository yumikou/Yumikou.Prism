using System;
using System.Collections.Generic;
using System.Text;
using Prism.Mvvm;
using Prism.Regions;

namespace StackNavigation.ViewModel
{
    public class RegionStackNavigationBase : BindableBase, INavigationAware, IRegionMemberLifetime
    {
        private bool _isNewBuild = true;
        protected string _stackPageId = "";

        public bool KeepAlive { get; set; } = true;

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.TryGetValue<string>("StackPageId", out string stackPageId))
            { 
                return _stackPageId == stackPageId;
            }
            return false;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
            if (navigationContext.NavigationType == NavigationType.GoBack)
            {
                KeepAlive = false;
            }
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (_isNewBuild)
            {
                _isNewBuild = false;
                if (navigationContext.Parameters.TryGetValue<string>("StackPageId", out string stackPageId)) 
                {
                    _stackPageId = stackPageId;
                }
            }
        }
    }
}
