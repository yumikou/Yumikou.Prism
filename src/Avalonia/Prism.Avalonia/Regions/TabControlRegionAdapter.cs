using Avalonia.Controls;
using Prism.Regions.Behaviors;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Regions
{
    public class TabControlRegionAdapter : RegionAdapterBase<TabControl>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TabControlRegionAdapter"/>.
        /// </summary>
        /// <param name="regionBehaviorFactory">The factory used to create the region behaviors to attach to the created regions.</param>
        public TabControlRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, TabControl regionTarget)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            // Add the behavior that syncs the items source items with the rest of the items
            region.Behaviors.Add(TabControlSyncBehavior.BehaviorKey, new TabControlSyncBehavior()
            {
                HostControl = regionTarget
            });

            base.AttachBehaviors(region, regionTarget);
        }

        protected override IRegion CreateRegion()
        {
            return new Region();
        }
    }
}
