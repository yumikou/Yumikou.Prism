using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prism.Regions
{
    /// <summary>
    /// Provides journaling of current, back, and forward navigation within regions.
    /// </summary>
    public interface IRegionNavigationJournal
    {
        /// <summary>
        /// Gets the current navigation entry of the content that is currently displayed.
        /// </summary>
        /// <value>The current entry.</value>
        IRegionNavigationJournalEntry CurrentEntry { get; }

        /// <summary>
        /// Gets or sets the target that implements INavigateAsync.
        /// </summary>
        /// <value>The INavigate implementation.</value>
        /// <remarks>
        /// This is set by the owner of this journal.
        /// </remarks>
        IRegionNavigationService NavigationTarget { get; set; }

        bool CanGoBack();

        bool CanGoBack(Func<IRegionNavigationJournalEntry, int, bool> gobackPredicate);

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history, or does nothing if no entry exists in back navigation.
        /// </summary>
        bool GoBack();

        bool GoBack(Func<IRegionNavigationJournalEntry, int, bool> gobackPredicate);

        bool CanGoForward();

        /// <summary>
        /// Navigates to the most recent entry in the forward navigation history, or does nothing if no entry exists in forward navigation.
        /// </summary>
        bool GoForward();

        /// <summary>
        /// Records the navigation to the entry..
        /// </summary>
        /// <param name="entry">The entry to record.</param>
        /// <param name="persistInHistory">Keep Navigation object in memory when OnNavigationFrom is called</param>
        void RecordNavigation(IRegionNavigationJournalEntry entry, NavigationType navigationType);

        /// <summary>
        /// Clears the journal of current, back, and forward navigation histories.
        /// </summary>
        void Clear();
    }
}
