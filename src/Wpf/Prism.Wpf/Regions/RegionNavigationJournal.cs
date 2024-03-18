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
        private bool isNavigatingInternal = false;

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
                return e.PersistInHistoryType == PersistInHistoryType.InHistory;
            });
        }

        public bool CanGoBack(Func<IRegionNavigationJournalEntry, int, bool> gobackPredicate)
        {
            return CanGoBackInternal(gobackPredicate, out List<IRegionNavigationJournalEntry> skippedBackStackList, out IRegionNavigationJournalEntry goBackEntry);
        }

        internal bool CanGoBackInternal(Func<IRegionNavigationJournalEntry, int, bool> gobackPredicate, out List<IRegionNavigationJournalEntry> skippedBackStackList, out IRegionNavigationJournalEntry goBackEntry)
        {
            int depth = 0;
            skippedBackStackList = new List<IRegionNavigationJournalEntry>();

            while (true)
            {
                if (depth >= this.backStack.Count)
                {
                    goBackEntry = null;
                    return false;
                }

                IRegionNavigationJournalEntry entry = this.backStack.ElementAt(depth);
                if (gobackPredicate is not null && !gobackPredicate.Invoke(entry, depth))
                {
                    skippedBackStackList.Add(entry);
                }
                else
                {
                    goBackEntry = entry;
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
                return e.PersistInHistoryType == PersistInHistoryType.InHistory; ;
            });
        }

        public bool GoBack(Func<IRegionNavigationJournalEntry, int, bool> gobackPredicate)
        {
            if (CanGoBackInternal(gobackPredicate, out List<IRegionNavigationJournalEntry> skippedBackStackList, out IRegionNavigationJournalEntry goBackEntry))
            {
                bool naviFlag = false;
                this.isNavigatingInternal = true;
                this.NavigationTarget.RequestNavigate(
                    goBackEntry.Uri,
                    nr =>
                    {
                        this.isNavigatingInternal = false;
                        if (nr.Result) //导航成功
                        {
                            if (nr.ResultEntry is null)
                            {
                                throw new NullReferenceException("导航成功的结果不能为空！");
                            }

                            foreach (var skippedEntry in skippedBackStackList)
                            {
                                RecordNavigation(skippedEntry, NavigationType.GoBack);

                                // 堆栈导航返回时，在inactive和从历史堆栈直接移除时，都需要删除view
                                var contract = UriParsingHelper.GetContract(skippedEntry.Uri);
                                var candidateType = _container.GetRegistrationType(contract);
                                if (candidateType is not null && RegionHelper.IsStackViewType(candidateType))
                                {
                                    if (skippedEntry.AssociatedView is not null && skippedEntry.AssociatedView.IsAlive) // IsPersistInHistory为false时的goforward，不会创建view直接跳过entry，这时候再返回跳过关联的view是空
                                    {
                                        NavigationTarget.Region.Remove(skippedEntry.AssociatedView.Target);
                                    }
                                }
                            }

                            RecordNavigation(nr.ResultEntry, NavigationType.GoBack);
                        }
                        naviFlag = nr.Result;
                    },
                    goBackEntry.Parameters,
                    goBackEntry.AssociatedView,
                    NavigationType.GoBack);
                return naviFlag;
            }
            return false;
        }

        public bool CanGoForward()
        {
            return CanGoForwardInternal(e =>
            {
                return e.PersistInHistoryType == PersistInHistoryType.InHistory || e.PersistInHistoryType == PersistInHistoryType.InHistoryAndSkipGoBack;
            },
            out List<IRegionNavigationJournalEntry> skippedForwardStackList,
            out IRegionNavigationJournalEntry goForwardEntry);
        }

        internal bool CanGoForwardInternal(Func<IRegionNavigationJournalEntry, bool> goforwardPredicate, out List<IRegionNavigationJournalEntry> skippedForwardStackList, out IRegionNavigationJournalEntry goForwardEntry)
        {
            int depth = 0;
            skippedForwardStackList = new List<IRegionNavigationJournalEntry>();

            while (true)
            {
                if (depth >= this.forwardStack.Count)
                {
                    goForwardEntry = null;
                    return false;
                }

                IRegionNavigationJournalEntry entry = this.forwardStack.ElementAt(depth);
                if (goforwardPredicate is not null && !goforwardPredicate.Invoke(entry))
                {
                    skippedForwardStackList.Add(entry);
                }
                else
                {
                    goForwardEntry = entry;
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
            if (CanGoForwardInternal(e =>
            {
                return e.PersistInHistoryType == PersistInHistoryType.InHistory || e.PersistInHistoryType == PersistInHistoryType.InHistoryAndSkipGoBack;
            },
            out List<IRegionNavigationJournalEntry> skippedForwardStackList,
            out IRegionNavigationJournalEntry goForwardEntry))
            {
                bool naviFlag = false;
                this.isNavigatingInternal = true;
                this.NavigationTarget.RequestNavigate(
                    goForwardEntry.Uri,
                    nr =>
                    {
                        this.isNavigatingInternal = false;
                        if (nr.Result) //导航成功
                        {
                            if (nr.ResultEntry is null)
                            {
                                throw new NullReferenceException("导航成功的结果不能为空！");
                            }

                            foreach (var skippedEntry in skippedForwardStackList)
                            {
                                RecordNavigation(skippedEntry, NavigationType.GoForward);
                            }

                            RecordNavigation(nr.ResultEntry, NavigationType.GoForward);
                        }
                        naviFlag = nr.Result;
                    },
                    goForwardEntry.Parameters,
                    goForwardEntry.AssociatedView,
                    NavigationType.GoForward);
                return naviFlag;
            }
            return false;
        }

        /// <summary>
        /// Records the navigation to the entry..
        /// </summary>
        /// <param name="entry">The entry to record.</param>
        /// <param name="persistInHistory">Determine if the view is added to the back stack or excluded from the history.</param>
        public void RecordNavigation(IRegionNavigationJournalEntry entry, NavigationType navigationType)
        {
            if (this.isNavigatingInternal) return;

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

                if (entry.PersistInHistoryType != PersistInHistoryType.NotInHistory)
                {
                    CurrentEntry = entry;
                }
                else
                {
                    CurrentEntry = null;
                }
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
