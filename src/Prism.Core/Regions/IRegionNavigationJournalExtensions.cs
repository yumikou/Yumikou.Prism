using System;
using System.Collections.Generic;
using System.Text;
using Prism.Common;

namespace Prism.Regions
{
    public static class IRegionNavigationJournalExtensions
    {
        public static bool CanGoBack(this IRegionNavigationJournal journal, int depth)
        {
            return journal.CanGoBack((entry, index) =>
            {
                return index == depth;
            });
        }

        public static bool CanGoBack(this IRegionNavigationJournal journal, string viewName)
        {
            return journal.CanGoBack((entry, index) =>
            {
                var contract = UriParsingHelper.GetContract(entry.Uri);
                return contract == viewName;
            });
        }

        public static bool GoBack(this IRegionNavigationJournal journal, int depth)
        {
            return journal.GoBack((entry, index) =>
            {
                return index == depth;
            });
        }

        public static bool GoBack(this IRegionNavigationJournal journal, string viewName)
        {
            return journal.GoBack((entry, index) =>
            {
                var contract = UriParsingHelper.GetContract(entry.Uri);
                return contract == viewName;
            });
        }
    }
}
