using System;
using System.Collections.Generic;
using System.Text;

namespace Prism.Regions
{
    public interface IRegionRequestCreateService : IRequestCreate
    {
        /// <summary>
        /// Gets or sets the region owning this service.
        /// </summary>
        /// <value>A Region.</value>
        IRegion Region { get; set; }
    }
}
