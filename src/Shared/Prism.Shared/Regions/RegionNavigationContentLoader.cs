using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Prism.Common;
using Prism.Ioc;
using Prism.Ioc.Internals;
using Prism.Properties;

#if _Avalonia_
using Avalonia.Controls;
#elif _Wpf_
using System.Windows;
#endif

namespace Prism.Regions
{
    /// <summary>
    /// Implementation of <see cref="IRegionNavigationContentLoader"/> that relies on a <see cref="IContainerProvider"/>
    /// to create new views when necessary.
    /// </summary>
    public class RegionNavigationContentLoader : IRegionNavigationContentLoader
    {
        private readonly IContainerExtension _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionNavigationContentLoader"/> class with a service locator.
        /// </summary>
        /// <param name="container">The <see cref="IContainerExtension" />.</param>
        public RegionNavigationContentLoader(IContainerExtension container)
        {
            _container = container;
        }

        /// <summary>
        /// Gets the view to which the navigation request represented by <paramref name="navigationContext"/> applies.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="navigationContext">The context representing the navigation request.</param>
        /// <returns>
        /// The view to be the target of the navigation request.
        /// </returns>
        /// <remarks>
        /// If none of the views in the region can be the target of the navigation request, a new view
        /// is created and added to the region.
        /// </remarks>
        /// <exception cref="ArgumentException">when a new view cannot be created for the navigation request.</exception>
        public object LoadContent(IRegion region, NavigationContext navigationContext)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            if (navigationContext == null)
                throw new ArgumentNullException(nameof(navigationContext));

            if (navigationContext.NavigationType == NavigationType.GoBack 
                && navigationContext.AssociatedView is not null && navigationContext.AssociatedView.IsAlive 
                && RegionHelper.GetLifetimeType(navigationContext.AssociatedView.Target) == RegionMemberLifetimeType.Stack)
            {
                if (region.Views.Contains(navigationContext.AssociatedView.Target))
                    return navigationContext.AssociatedView.Target;
                else
                    throw new InvalidOperationException("导航堆栈中记录的view在Region中找不到了");
            }
            else
            {
                string candidateTargetContract = UriParsingHelper.GetContract(navigationContext.Uri);
                var candidates = GetCandidatesFromRegion(region, candidateTargetContract);
                var acceptingCandidates =
                    candidates.Where(
                        v =>
                        {
                            if (v is INavigationAware navigationAware && !navigationAware.IsNavigationTarget(navigationContext))
                            {
                                return false;
                            }

#if _Avalonia_
                            if (!(v is Control control))
#elif _Wpf_
                            if (!(v is FrameworkElement frameworkElement))
#endif
                            {
                                return true;
                            }

#if _Avalonia_
                            navigationAware = control.DataContext as INavigationAware;
#elif _Wpf_
                            navigationAware = frameworkElement.DataContext as INavigationAware;
#endif
                            return navigationAware == null || navigationAware.IsNavigationTarget(navigationContext);
                        });

                var view = acceptingCandidates.FirstOrDefault();

                if (view != null)
                {
                    navigationContext.AssociatedView = new WeakReference(view);
                    return view;
                }

                // Add a new view to region
                var createResult = region.RequestCreateService.RequestCreate(navigationContext.Uri, navigationContext.SourceParameters, false);
                if (createResult.Result && createResult.Context.AssociatedView.IsAlive)
                {
                    navigationContext.AssociatedView = createResult.Context.AssociatedView;
                    return createResult.Context.AssociatedView.Target;
                }
                else
                {
                    throw createResult.Error ?? new Exception("An unknown error occurred while creating a view for the region!");
                }
            }
        }

        /// <summary>
        /// Returns the set of candidates that may satisfy this navigation request.
        /// </summary>
        /// <param name="region">The region containing items that may satisfy the navigation request.</param>
        /// <param name="candidateNavigationContract">The candidate navigation target as determined by <see cref="GetContractFromNavigationContext"/></param>
        /// <returns>An enumerable of candidate objects from the <see cref="IRegion"/></returns>
        protected virtual IEnumerable<object> GetCandidatesFromRegion(IRegion region, string candidateNavigationContract)
        {
            return RegionHelper.GetNavigationCandidates(region, candidateNavigationContract);
        }
    }
}
