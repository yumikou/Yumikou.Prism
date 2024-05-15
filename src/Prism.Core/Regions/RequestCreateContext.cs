using System;
using System.Collections.Generic;
using System.Text;
using Prism.Common;

namespace Prism.Regions
{
    public class RequestCreateContext
    {
        /// <summary>
        /// Gets or sets the request add view to region service.
        /// </summary>
        /// <value>The navigation service.</value>
        public IRegionRequestCreateService RequestCreateService { get; private set; }

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

        public WeakReference AssociatedView { get; set; }

        public RequestCreateContext(IRegionRequestCreateService requestCreateService, Uri uri) : this(requestCreateService, uri, null)
        {

        }

        public RequestCreateContext(IRegionRequestCreateService requestCreateService, Uri uri, NavigationParameters navigationParameters) :this(requestCreateService, uri, navigationParameters, null)
        {

        }

        public RequestCreateContext(IRegionRequestCreateService requestCreateService, Uri uri, NavigationParameters navigationParameters, WeakReference associatedView)
        {
            RequestCreateService = requestCreateService;
            Uri = uri;
            SourceParameters = (navigationParameters ?? new NavigationParameters());
            Parameters = uri != null ? UriParsingHelper.ParseQuery(uri) : null;
            GetNavigationParameters(navigationParameters);
            AssociatedView = associatedView;
        }

        private void GetNavigationParameters(NavigationParameters navigationParameters)
        {
            if (Parameters is null)
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
