using System;
using System.Collections.Generic;
using Prism.Common;

namespace Prism.Regions
{
    /// <summary>
    /// Encapsulates information about a navigation request.
    /// </summary>
    public class NavigationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationContext"/> class for a region name and a 
        /// <see cref="Uri"/>.
        /// </summary>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="uri">The Uri.</param>
        public NavigationContext(IRegionNavigationService navigationService, Uri uri, NavigationType navigationType) : this(navigationService, uri, null, null, navigationType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationContext"/> class for a region name and a 
        /// <see cref="Uri"/>.
        /// </summary>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="navigationParameters">The navigation parameters.</param>
        /// <param name="uri">The Uri.</param>
        public NavigationContext(IRegionNavigationService navigationService, Uri uri, NavigationParameters navigationParameters, WeakReference associatedView, NavigationType navigationType)
        {
            NavigationService = navigationService;
            Uri = uri;
            SourceParameters = (navigationParameters ?? new NavigationParameters());
            Parameters = uri != null ? UriParsingHelper.ParseQuery(uri) : null;
            GetNavigationParameters(navigationParameters);
            AssociatedView = associatedView;
            NavigationType = navigationType;
        }

        /// <summary>
        /// Gets the region navigation service.
        /// </summary>
        /// <value>The navigation service.</value>
        public IRegionNavigationService NavigationService { get; private set; }

        /// <summary>
        /// Gets the navigation URI.
        /// </summary>
        /// <value>The navigation URI.</value>
        public Uri Uri { get; private set; }

        /// <summary>
        /// Gets the <see cref="NavigationParameters"/> unmodified parameters.
        /// </summary>
        public NavigationParameters SourceParameters { get; private set; }

        /// <summary>
        /// Gets the <see cref="NavigationParameters"/> extracted from the URI and the object parameters passed in navigation.
        /// </summary>
        /// <value>The URI query.</value>
        public NavigationParameters Parameters { get; private set; }

        public NavigationType NavigationType { get; private set; }

        public WeakReference AssociatedView { get; set; }

        private void GetNavigationParameters(NavigationParameters navigationParameters)
        {
            if (Parameters == null || NavigationService == null || NavigationService.Region == null)
            {
                Parameters = new NavigationParameters();
                return;
            }

            if (navigationParameters != null)
            {
                foreach (KeyValuePair<string, object> navigationParameter in navigationParameters)
                {
                    this.Parameters.Add(navigationParameter.Key, navigationParameter.Value);
                }
            }
        }
    }
}
