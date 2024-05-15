using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Ioc;
using Prism.Ioc.Internals;

namespace Prism.Common
{
    internal static class RegionHelper
    {
        public static bool InactiveViewShouldKeepAlive(object inactiveView, NavigationType? navigationType)
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

        public static RegionMemberLifetimeType GetLifetimeType(object view)
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

        /// <summary>
        /// Returns the set of candidates that may satisfy this navigation request.
        /// </summary>
        /// <param name="region">The region containing items that may satisfy the navigation request.</param>
        /// <param name="candidateNavigationContract">The candidate navigation target as determined by <see cref="GetContractFromNavigationContext"/></param>
        /// <returns>An enumerable of candidate objects from the <see cref="IRegion"/></returns>
        public static IEnumerable<object> GetNavigationCandidates(IRegion region, string candidateNavigationContract)
        {
            if (region is null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (string.IsNullOrEmpty(candidateNavigationContract))
            {
                throw new ArgumentNullException(nameof(candidateNavigationContract));
            }

            var contractCandidates = GetCandidatesFromRegionViews(region, candidateNavigationContract);

            if (!contractCandidates.Any())
            {
                var matchingType = ContainerLocator.Current.GetRegistrationType(candidateNavigationContract);
                if (matchingType is null)
                {
#if _Avalonia_
                    return Array.Empty<object>();
#elif _Wpf_
                    return ArrayEx.Empty<object>();
#endif
                }

                return GetCandidatesFromRegionViews(region, matchingType.FullName);
            }

            return contractCandidates;
        }

        private static IEnumerable<object> GetCandidatesFromRegionViews(IRegion region, string candidateNavigationContract)
        {
            return region.Views.Where(v => ViewIsMatch(v.GetType(), candidateNavigationContract));
        }

        private static bool ViewIsMatch(Type viewType, string navigationSegment)
        {
            var names = new[] { viewType.Name, viewType.FullName };
            return names.Any(x => x.Equals(navigationSegment, StringComparison.Ordinal));
        }
    }
}
