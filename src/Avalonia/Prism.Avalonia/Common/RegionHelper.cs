using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Common
{
    internal static class RegionMemberHelper
    {
        private static HashSet<Type> viewTypesForStackNavigation = new HashSet<Type>();

        internal static bool AddViewTypeForStackNavigation(Type viewType)
        {
            return viewTypesForStackNavigation.Add(viewType);
        }

        internal static bool IsStackViewType(Type viewType)
        {
            return viewTypesForStackNavigation.Contains(viewType);
        }

        internal static bool InactiveViewShouldKeepAlive(object inactiveView, NavigationType navigationType)
        {
            if (IsStackViewType(inactiveView.GetType()))
            {
                return navigationType != NavigationType.GoBack;
            }
            else
            {
                IRegionMemberLifetime lifetime = MvvmHelpers.GetImplementerFromViewOrViewModel<IRegionMemberLifetime>(inactiveView);
                if (lifetime != null)
                {
                    return lifetime.KeepAlive;
                }

                RegionMemberLifetimeAttribute lifetimeAttribute = MvvmHelpers.GetAttributeFromViewOrViewModel<RegionMemberLifetimeAttribute>(inactiveView, true);
                if (lifetimeAttribute != null)
                {
                    return lifetimeAttribute.KeepAlive;
                }

                return true;
            }
        }
    }
}
