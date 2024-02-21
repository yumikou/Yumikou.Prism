using Prism.Common;
using Prism.Ioc;
using Prism.Ioc.Internals;
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
        private readonly IContainerExtension _container;
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

        public RegionNavigationJournal(IContainerExtension container)
        {
            _container = container;
        }

        public bool CanGoBack()
        {
            return CanGoBack((e, i) =>
            {
                return e.IsPersistInHistory;
            });
        }

        public bool CanGoBack(Func<IRegionNavigationJournalEntry, int, bool> gobackPredicate)
        {
            int depth = 0;

            while (true)
            {
                if (depth >= this.backStack.Count) { return false; }

                IRegionNavigationJournalEntry entry = this.backStack.ElementAt(depth);
                if (gobackPredicate is not null && !gobackPredicate.Invoke(entry, depth))
                { 
                
                }
                else
                {
                    return true;
                }
                depth++;
            }
        }

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history, or does nothing if no entry exists in back navigation.
        /// </summary>
        public bool GoBack()
        {
            return GoBack((e, i) =>
            {
                return e.IsPersistInHistory;
            });
        }

        public bool GoBack(Func<IRegionNavigationJournalEntry, int, bool> gobackPredicate)
        {
            int depth = 0;

            while (true)
            {
                if (this.backStack.Count <= 0) { return false; }

                IRegionNavigationJournalEntry entry = this.backStack.Peek();
                if (gobackPredicate is not null && !gobackPredicate.Invoke(entry, depth))
                {
                    RecordNavigation(entry, NavigationType.GoBack);

                    // 堆栈导航返回时，在inactive和从历史堆栈直接移除时，都需要删除view
                    var contract = UriParsingHelper.GetContract(entry.Uri);
                    var candidateType = _container.GetRegistrationType(contract);
                    if (candidateType is not null && RegionHelper.IsStackViewType(candidateType))
                    {
                        if (entry.AssociatedView is null || !entry.AssociatedView.IsAlive) throw new InvalidOperationException("堆栈导航历史中的View被提前回收了");
                        NavigationTarget.Region.Remove(entry.AssociatedView.Target);
                    }
                }
                else
                {
                    this.NavigationTarget.RequestNavigate(
                        entry.Uri,
                        nr => { },
                        entry.Parameters,
                        entry.AssociatedView,
                        NavigationType.GoBack);
                    return true;
                }
                depth++;
            }
        }

        public bool CanGoForward()
        {
            int depth = 0;
            while (true)
            {
                if (depth >= this.forwardStack.Count) { return false; }

                IRegionNavigationJournalEntry entry = this.forwardStack.ElementAt(depth);
                if (!entry.IsPersistInHistory)
                {

                }
                else
                {
                    return true;
                }
                depth++;
            }
        }

        /// <summary>
        /// Navigates to the most recent entry in the forward navigation history, or does nothing if no entry exists in forward navigation.
        /// </summary>
        public bool GoForward()
        {
            while (true)
            {
                if (this.forwardStack.Count <= 0) { return false; }

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
                    return true;
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
