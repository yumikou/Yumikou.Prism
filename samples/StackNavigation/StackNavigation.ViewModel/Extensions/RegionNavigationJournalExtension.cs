using System;
using System.Collections.Generic;
using System.Text;

namespace Prism.Regions
{
    internal static class RegionNavigationJournalExtension
    {
        public static void GoBack(this IRegionNavigationJournal journal, string viewName)
        {
            while (journal.CanGoBack && (journal.CurrentEntry == null || GetJournalEntryViewName(journal.CurrentEntry.Uri) != viewName))
            {
                journal.GoBack();
            }
        }

        private static string GetJournalEntryViewName(Uri uri)
        {
            if (uri != null)
            {
                if (uri.OriginalString.Contains("?"))
                {
                    return uri.OriginalString.Split('?')[0];
                }
                else
                {
                    return uri.OriginalString;
                }
            }
            return null;
        }
    }
}
