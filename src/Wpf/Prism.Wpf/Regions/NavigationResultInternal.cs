using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Regions
{
    internal class NavigationResultInternal : NavigationResult
    {
        public IRegionNavigationJournalEntry ResultEntry { get; private set; }

        public NavigationResultInternal(NavigationContext context, bool result, IRegionNavigationJournalEntry resultEntry) : base(context, result)
        {
            ResultEntry = resultEntry;
        }
    }
}
