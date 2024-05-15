using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Properties;
using System.Collections.Specialized;
using Avalonia.Controls;

namespace Prism.Regions
{
    public class TransitioningContentControlRegionAdapter : RegionAdapterBase<TransitioningContentControl>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ContentControlRegionAdapter"/>.
        /// </summary>
        /// <param name="regionBehaviorFactory">The factory used to create the region behaviors to attach to the created regions.</param>
        public TransitioningContentControlRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        /// <summary>
        /// Adapts a <see cref="ContentControl"/> to an <see cref="IRegion"/>.
        /// </summary>
        /// <param name="region">The new region being used.</param>
        /// <param name="regionTarget">The object to adapt.</param>
        protected override void Adapt(IRegion region, TransitioningContentControl regionTarget)
        {
            if (regionTarget == null)
                throw new ArgumentNullException(nameof(regionTarget));

            bool contentIsSet = regionTarget.Content != null;
            contentIsSet = contentIsSet || regionTarget[Avalonia.Controls.ContentControl.ContentProperty] != null;

            if (contentIsSet)
                throw new InvalidOperationException(Resources.ContentControlHasContentException);

            region.ActiveViews.NavigationCollectionChanged += (s, e) =>
            {
                var aView = region.ActiveViews.FirstOrDefault();
                if (e.Action != NotifyCollectionChangedAction.Add && e.Action != NotifyCollectionChangedAction.Reset) return;
                if (regionTarget.Content == aView) return;

                if (e.NavigationType == NavigationType.GoBack)
                {
                    regionTarget.IsTransitionReversed = true;
                    regionTarget.Content = aView;
                }
                else //goForward或Navigate或非导航加载(NavigationType为null)
                {
                    regionTarget.IsTransitionReversed = false;
                    regionTarget.Content = aView;
                }
            };
           
            region.Views.CollectionChanged +=
                (sender, e) =>
                {
                    if (e.Action == NotifyCollectionChangedAction.Add && region.ActiveViews.Count() == 0)
                    {
                        region.Activate(e.NewItems[0], null);
                    }
                };
        }

        /// <summary>
        /// Creates a new instance of <see cref="SingleActiveRegion"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="SingleActiveRegion"/>.</returns>
        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }
    }
}
