using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Common
{
    internal static class RegionHelper
    {
        internal static bool InactiveViewShouldKeepAlive(object inactiveView, NavigationType? navigationType)
        {
            RegionMemberLifetimeType lifetimeType = GetLifetimeType(inactiveView);
            if (lifetimeType == RegionMemberLifetimeType.Transient)
            {
                return false;
            }
            else if (lifetimeType == RegionMemberLifetimeType.KeepAlive)
            {
                return true;
            }
            else // Stack
            {
                return navigationType != NavigationType.GoBack;
            }
        }

        internal static RegionMemberLifetimeType GetLifetimeType(object view)
        {
            IRegionMemberLifetime lifetime = MvvmHelpers.GetImplementerFromViewOrViewModel<IRegionMemberLifetime>(view);
            if (lifetime != null)
            {
                return lifetime.RegionMemberLifetimeType;
            }

            RegionMemberLifetimeAttribute lifetimeAttribute = MvvmHelpers.GetAttributeFromViewOrViewModel<RegionMemberLifetimeAttribute>(view, true);
            if (lifetimeAttribute != null)
            {
                return lifetimeAttribute.RegionMemberLifetimeType;
            }

            return RegionMemberLifetimeType.KeepAlive;
        }
    }
}
