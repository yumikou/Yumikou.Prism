using System;
using System.Collections.Generic;
using System.Text;
using Prism.Regions;
using Prism.Navigation;

namespace NestedRegion.ViewModel
{
    public class BN1ViewModel : ViewModelHeaderBase, IRegionMemberLifetime, IDestructible
    {
        public RegionMemberLifetimeType RegionMemberLifetimeType { get; } = RegionMemberLifetimeType.Transient;

        public void Destroy()
        {

        }
    }
}
