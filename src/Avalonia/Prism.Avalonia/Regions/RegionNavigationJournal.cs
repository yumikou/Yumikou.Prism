using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Regions
{
    /// <summary>
    /// Provides journaling of current, back, and forward navigation within regions.
    /// </summary>
    public class RegionNavigationJournal : IRegionNavigationJournal
    {
        private Stack<IRegionNavigationJournalEntry> backStack = new Stack<IRegionNavigationJournalEntry>();
        private Stack<IRegionNavigationJournalEntry> forwardStack = new Stack<IRegionNavigationJournalEntry>();

        /// <summary>
        /// Gets or sets the target that implements INavigate.
        /// </summary>
        /// <value>The INavigate implementation.</value>
        /// <remarks>
        /// This is set by the owner of this journal.
        /// </remarks>
        public IRegionNavigationService NavigationTarget { get; set; }

        /// <summary>
        /// Gets the current navigation entry of the content that is currently displayed.
        /// </summary>
        /// <value>The current entry.</value>
        public IRegionNavigationJournalEntry CurrentEntry { get; private set; }

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in the back navigation history.
        /// </summary>
        /// <value><c>true</c> if the journal can go back; otherwise, <c>false</c>.</value>
        public bool CanGoBack
        {
            get
            {
                return this.backStack.Any(entry => { return entry.IsPersistInHistory; });
            }
        }

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in the forward navigation history.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can go forward; otherwise, <c>false</c>.
        /// </value>
        public bool CanGoForward
        {
            get
            {
                return this.forwardStack.Any(entry => { return entry.IsPersistInHistory; });
            }
        }

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history, or does nothing if no entry exists in back navigation.
        /// </summary>
        public void GoBack()
        {
            if (this.CanGoBack)
            {
                while(true)
                {
                    IRegionNavigationJournalEntry entry = this.backStack.Peek();

                    if (!entry.IsPersistInHistory)
                    {
                        RecordNavigation(entry, NavigationType.GoBack);
                        // TODO: 如果是Stack类型，同时删除缓存的view
                    }
                    else
                    {
                        this.NavigationTarget.RequestNavigate(
                            entry.Uri,
                            nr => { },
                            entry.Parameters,
                            entry.AssociatedView,
                            NavigationType.GoBack);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Navigates to the most recent entry in the forward navigation history, or does nothing if no entry exists in forward navigation.
        /// </summary>
        public void GoForward()
        {
            if (this.CanGoForward)
            {
                while(true)
                {
                    IRegionNavigationJournalEntry entry = this.forwardStack.Peek();

                    if (!entry.IsPersistInHistory)
                    {
                        RecordNavigation(entry, NavigationType.GoForward);
                    }
                    else
                    {
                        this.NavigationTarget.RequestNavigate(
                            entry.Uri,
                            nr => { },
                            entry.Parameters,
                            entry.AssociatedView,
                            NavigationType.GoForward);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Records the navigation to the entry..
        /// </summary>
        /// <param name="entry">The entry to record.</param>
        /// <param name="persistInHistory">Determine if the view is added to the back stack or excluded from the history.</param>
        public void RecordNavigation(IRegionNavigationJournalEntry entry, NavigationType navigationType)
        {
            if (navigationType == NavigationType.GoBack)
            {
                if (this.CurrentEntry != null)
                {
                    this.forwardStack.Push(this.CurrentEntry);
                }

                this.backStack.Pop();
                this.CurrentEntry = entry;
            }
            else if (navigationType == NavigationType.GoForward)
            {
                if (this.CurrentEntry != null)
                {
                    this.backStack.Push(this.CurrentEntry);
                }

                this.forwardStack.Pop();
                this.CurrentEntry = entry;
            }
            else //导航到新页面
            {
                if (this.CurrentEntry != null)
                {
                    this.backStack.Push(this.CurrentEntry);
                }

                this.forwardStack.Clear();

                CurrentEntry = entry;
            }
        }

        /// <summary>
        /// Clears the journal of current, back, and forward navigation histories.
        /// </summary>
        public void Clear()
        {
            this.CurrentEntry = null;
            this.backStack.Clear();
            this.forwardStack.Clear();
        }

    }
}
