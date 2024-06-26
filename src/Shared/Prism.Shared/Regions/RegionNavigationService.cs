using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using Prism.Common;
using Prism.Ioc;
using Prism.Properties;

#if _Avalonia_
using Avalonia.Controls;
#elif _Wpf_
using System.Windows;
#endif

namespace Prism.Regions
{
    /// <summary>
    /// Provides navigation for regions.
    /// </summary>
    public class RegionNavigationService : IRegionNavigationService
    {
        private readonly IContainerProvider _container;
        private readonly IRegionNavigationContentLoader _regionNavigationContentLoader;
        private NavigationContext _currentNavigationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionNavigationService"/> class.
        /// </summary>
        /// <param name="container">The <see cref="IContainerExtension" />.</param>
        /// <param name="regionNavigationContentLoader">The navigation target handler.</param>
        /// <param name="journal">The journal.</param>
        public RegionNavigationService(IContainerExtension container, IRegionNavigationContentLoader regionNavigationContentLoader, IRegionNavigationJournal journal)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _regionNavigationContentLoader = regionNavigationContentLoader ?? throw new ArgumentNullException(nameof(regionNavigationContentLoader));
            Journal = journal ?? throw new ArgumentNullException(nameof(journal));
            Journal.NavigationTarget = this;
        }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        /// <value>The region.</value>
        public IRegion Region { get; set; }

        /// <summary>
        /// Gets the journal.
        /// </summary>
        /// <value>The journal.</value>
        public IRegionNavigationJournal Journal { get; private set; }

        /// <summary>
        /// Raised when the region is about to be navigated to content.
        /// </summary>
        public event EventHandler<RegionNavigationEventArgs> Navigating;

        private void RaiseNavigating(NavigationContext navigationContext)
        {
            Navigating?.Invoke(this, new RegionNavigationEventArgs(navigationContext));
        }

        /// <summary>
        /// Raised when the region is navigated to content.
        /// </summary>
        public event EventHandler<RegionNavigationEventArgs> Navigated;

        private void RaiseNavigated(NavigationContext navigationContext)
        {
            Navigated?.Invoke(this, new RegionNavigationEventArgs(navigationContext));
        }

        /// <summary>
        /// Raised when a navigation request fails.
        /// </summary>
        public event EventHandler<RegionNavigationFailedEventArgs> NavigationFailed;

        private void RaiseNavigationFailed(NavigationContext navigationContext, Exception error)
        {
            NavigationFailed?.Invoke(this, new RegionNavigationFailedEventArgs(navigationContext, error));
        }

        /// <summary>
        /// Initiates navigation to the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="navigationCallback">A callback to execute when the navigation request is completed.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is marshalled to callback")]
        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback, NavigationType navigationType = NavigationType.Navigate)
        {
            RequestNavigate(target, navigationCallback, null, navigationType);
        }

        /// <summary>
        /// Initiates navigation to the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="navigationCallback">A callback to execute when the navigation request is completed.</param>
        /// <param name="navigationParameters">The navigation parameters specific to the navigation request.</param>
        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters, NavigationType navigationType = NavigationType.Navigate)
        {
            RequestNavigate(target, navigationCallback, navigationParameters, null, navigationType);
        }

        /// <summary>
        /// Initiates navigation to the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="navigationCallback">A callback to execute when the navigation request is completed.</param>
        /// <param name="navigationParameters">The navigation parameters specific to the navigation request.</param>
        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters, WeakReference associatedView, NavigationType navigationType = NavigationType.Navigate)
        {
            if (navigationCallback == null)
                throw new ArgumentNullException(nameof(navigationCallback));

            try
            {
                DoNavigate(target, navigationCallback, navigationParameters, associatedView, navigationType);
            }
            catch (Exception e)
            {
                NotifyNavigationFailed(new NavigationContext(this, target, navigationParameters, associatedView, navigationType), navigationCallback, e);
            }
        }

        private void DoNavigate(Uri source, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters, WeakReference associatedView, NavigationType navigationType)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (Region == null)
                throw new InvalidOperationException(Resources.NavigationServiceHasNoRegion);

            _currentNavigationContext = new NavigationContext(this, source, navigationParameters, associatedView, navigationType);

            // starts querying the active views
            RequestCanNavigateFromOnCurrentlyActiveView(
                _currentNavigationContext,
                navigationCallback,
                Region.ActiveViews.ToArray(),
                0);
        }

        private void RequestCanNavigateFromOnCurrentlyActiveView(
                    NavigationContext navigationContext,
                    Action<NavigationResult> navigationCallback,
                    object[] activeViews,
                    int currentViewIndex)
        {
            if (currentViewIndex < activeViews.Length)
            {
                if (activeViews[currentViewIndex] is IConfirmNavigationRequest vetoingView)
                {
                    // the current active view implements IConfirmNavigationRequest, request confirmation
                    // providing a callback to resume the navigation request
                    vetoingView.ConfirmNavigationRequest(
                        navigationContext,
                        canNavigate =>
                        {
                            if (_currentNavigationContext == navigationContext && canNavigate)
                            {
                                RequestCanNavigateFromOnCurrentlyActiveViewModel(
                                    navigationContext,
                                    navigationCallback,
                                    activeViews,
                                    currentViewIndex);
                            }
                            else
                            {
                                NotifyNavigationFailed(navigationContext, navigationCallback, null);
                            }
                        });
                }
                else
                {
                    RequestCanNavigateFromOnCurrentlyActiveViewModel(
                        navigationContext,
                        navigationCallback,
                        activeViews,
                        currentViewIndex);
                }
            }
            else
            {
                ExecuteNavigation(navigationContext, activeViews, navigationCallback);
            }
        }

        private void RequestCanNavigateFromOnCurrentlyActiveViewModel(
            NavigationContext navigationContext,
            Action<NavigationResult> navigationCallback,
            object[] activeViews,
            int currentViewIndex)
        {
#if _Avalonia_
            if (activeViews[currentViewIndex] is Control control)
#elif _Wpf_
            if (activeViews[currentViewIndex] is FrameworkElement control)
#endif
            {
                if (control.DataContext is IConfirmNavigationRequest vetoingViewModel)
                {
                    // the data model for the current active view implements IConfirmNavigationRequest, request confirmation
                    // providing a callback to resume the navigation request
                    vetoingViewModel.ConfirmNavigationRequest(
                        navigationContext,
                        canNavigate =>
                        {
                            if (_currentNavigationContext == navigationContext && canNavigate)
                            {
                                RequestCanNavigateFromOnCurrentlyActiveView(
                                    navigationContext,
                                    navigationCallback,
                                    activeViews,
                                    currentViewIndex + 1);
                            }
                            else
                            {
                                NotifyNavigationFailed(navigationContext, navigationCallback, null);
                            }
                        });

                    return;
                }
            }

            RequestCanNavigateFromOnCurrentlyActiveView(
                navigationContext,
                navigationCallback,
                activeViews,
                currentViewIndex + 1);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is marshalled to callback")]
        private void ExecuteNavigation(NavigationContext navigationContext, object[] activeViews, Action<NavigationResult> navigationCallback)
        {
            try
            {
                NotifyActiveViewsNavigatingFrom(navigationContext, activeViews);

                Region.CanActivate = false; // 导航期间的视图激活行为在RegionNavigationService里手动处理，因此这里禁用了LoadContent内部在添加视图时的自动激活行为
                Region.CanDeactivate = false;
                object view = _regionNavigationContentLoader.LoadContent(Region, navigationContext);
                Region.CanDeactivate = true;
                Region.CanActivate = true;

                // Raise the navigating event just before activating the view.
                RaiseNavigating(navigationContext);

                Region.Activate(view, navigationContext.NavigationType);

                // Update the navigation journal before notifying others of navigation
                IRegionNavigationJournalEntry journalEntry = _container.Resolve<IRegionNavigationJournalEntry>();
                journalEntry.Uri = navigationContext.Uri;
                journalEntry.Parameters = navigationContext.Parameters;
                PersistInHistoryType persistInHistory = PersistInHistory(view);
                journalEntry.PersistInHistoryType = persistInHistory;
                journalEntry.AssociatedView = new WeakReference(view);

                // The view can be informed of navigation
                Action<INavigationAware> action = (n) => n.OnNavigatedTo(navigationContext);
                MvvmHelpers.ViewAndViewModelAction(view, action);

                Journal.RecordNavigation(journalEntry, navigationContext.NavigationType);
                
                navigationCallback(new NavigationResult(navigationContext, true, journalEntry));

                // Raise the navigated event when navigation is completed.
                RaiseNavigated(navigationContext);
            }
            catch (Exception e)
            {
                NotifyNavigationFailed(navigationContext, navigationCallback, e);
            }
        }

        private static PersistInHistoryType PersistInHistory(object view)
        {
            PersistInHistoryType persist = PersistInHistoryType.InHistory;
            MvvmHelpers.ViewAndViewModelAction<IJournalAware>(view, ija => { persist = ija.PersistInHistoryType(); });
            return persist;
        }

        private void NotifyNavigationFailed(NavigationContext navigationContext, Action<NavigationResult> navigationCallback, Exception e)
        {
            var navigationResult =
                e != null ? new NavigationResult(navigationContext, e) : new NavigationResult(navigationContext, false);

            navigationCallback(navigationResult);
            RaiseNavigationFailed(navigationContext, e);
        }

        private static void NotifyActiveViewsNavigatingFrom(NavigationContext navigationContext, object[] activeViews)
        {
            InvokeOnNavigationAwareElements(activeViews, (n) => n.OnNavigatedFrom(navigationContext));
        }

        private static void InvokeOnNavigationAwareElements(IEnumerable<object> items, Action<INavigationAware> invocation)
        {
            foreach (var item in items)
            {
                MvvmHelpers.ViewAndViewModelAction(item, invocation);
            }
        }
    }
}
