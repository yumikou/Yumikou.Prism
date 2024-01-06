using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Prism.Regions
{
    internal static class RegionManagerExtension
    {
        public static void RequestStackNavigate(this IRegionManager regionManager, string regionName, string target)
        {
            regionManager.RequestStackNavigate(regionName, new Uri(target, UriKind.RelativeOrAbsolute), nr => { }, null);
        }

        public static void RequestStackNavigate(this IRegionManager regionManager, string regionName, Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            if (navigationParameters is null)
            {
                navigationParameters = new NavigationParameters();
            }
            navigationParameters.Add("StackPageId", Guid.NewGuid().ToString());
            regionManager.RequestNavigate(regionName, target, navigationCallback, navigationParameters);
        }
    }
}
