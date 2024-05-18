using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Properties;
using System.Collections.Specialized;
using System.Diagnostics;
using Avalonia.Collections;
using Avalonia.Threading;

namespace Prism.Regions.Behaviors
{
    public class TabControlSyncBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        /// <summary>
        /// Name that identifies the TabControlSyncBehavior behavior in a collection of RegionsBehaviors. 
        /// </summary>
        public static readonly string BehaviorKey = "TabControlSyncBehavior";
        private bool _updatingActiveViewsInHostControlSelectionChanged;
        private TabControl _hostControl;

        /// <summary>
        /// Gets or sets the <see cref="AvaloniaObject"/> that the <see cref="IRegion"/> is attached to.
        /// </summary>
        /// <value>
        /// A <see cref="AvaloniaObject"/> that the <see cref="IRegion"/> is attached to.
        /// </value>
        /// <remarks>For this behavior, the host control must always be a <see cref="Selector"/> or an inherited class.</remarks>
        public AvaloniaObject HostControl
        {
            get
            {
                return this._hostControl;
            }

            set
            {
                this._hostControl = value as TabControl;
            }
        }

        /// <summary>
        /// Starts to monitor the <see cref="IRegion"/> to keep it in sync with the items of the <see cref="HostControl"/>.
        /// </summary>
        protected override void OnAttach()
        {
            bool itemsSourceIsSet = this._hostControl.ItemsSource != null;
            //itemsSourceIsSet = itemsSourceIsSet || this.hostControl.HasBinding(ItemsControl.ItemsSourceProperty);

            if (itemsSourceIsSet)
            {
                throw new InvalidOperationException(Resources.ItemsControlHasItemsSourceException);
            }

            this.SynchronizeItems();

            this._hostControl.SelectionChanged += this.HostControlSelectionChanged;
            this.Region.ActiveViews.NavigationCollectionChanged += this.ActiveViews_CollectionChanged;
            this.Region.Views.CollectionChanged += this.Views_CollectionChanged;
        }

        private void Views_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this._updatingActiveViewsInHostControlSelectionChanged)
            {
                // TODO: 在SelectionChanged事件中添加或删除Item都有可能会引发索引越界异常
                // 因此由SelectionChanged事件触发的添加或删除Item临时改为异步执行，这可能会导致非预期的行为，并不是合理的做法
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (e.Action == NotifyCollectionChangedAction.Add)
                    {
                        int startIndex = e.NewStartingIndex;
                        foreach (object newItem in e.NewItems)
                        {
                            this._hostControl.Items.Insert(startIndex++, newItem);
                        }
                    }
                    else if (e.Action == NotifyCollectionChangedAction.Remove)
                    {
                        foreach (object oldItem in e.OldItems)
                        {
                            this._hostControl.Items.Remove(oldItem);
                        }
                    }
                });
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                int startIndex = e.NewStartingIndex;
                foreach (object newItem in e.NewItems)
                {
                    this._hostControl.Items.Insert(startIndex++, newItem);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object oldItem in e.OldItems)
                {
                    this._hostControl.Items.Remove(oldItem);
                }
            }
        }

        private void SynchronizeItems()
        {
            List<object> existingItems = new List<object>();

            // Control must be empty before "Binding" to a region
            foreach (object childItem in this._hostControl.Items)
            {
                existingItems.Add(childItem);
            }

            foreach (object view in this.Region.Views)
            {
                this._hostControl.Items.Add(view);
            }

            foreach (object existingItem in existingItems)
            {
                this.Region.Add(existingItem);
            }
        }


        private void ActiveViews_CollectionChanged(object sender, NavigationNotifyCollectionChangedEventArgs e)
        {
            if (this._updatingActiveViewsInHostControlSelectionChanged)
            {
                // If we are updating the ActiveViews collection in the HostControlSelectionChanged, that 
                // means the user has set the SelectedItem or SelectedItems himself and we don't need to do that here now
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (this._hostControl.SelectedItem != null
                    && this._hostControl.SelectedItem != e.NewItems[0]
                    && this.Region.ActiveViews.Contains(this._hostControl.SelectedItem))
                {
                    this.Region.Deactivate(this._hostControl.SelectedItem, e.NavigationType);
                }

                this._hostControl.SelectedItem = e.NewItems[0];
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove &&
                     e.OldItems.Contains(this._hostControl.SelectedItem))
            {
                this._hostControl.SelectedItem = null;
            }
        }

        private void HostControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Record the fact that we are now updating active views in the HostControlSelectionChanged method. 
                // This is needed to prevent the ActiveViews_CollectionChanged() method from firing. 
                this._updatingActiveViewsInHostControlSelectionChanged = true;

                foreach (object item in e.RemovedItems)
                {
                    // check if the view is in both Views and ActiveViews collections (there may be out of sync)
                    if (this.Region.Views.Contains(item) && this.Region.ActiveViews.Contains(item))
                    {
                        this.Region.Deactivate(item, null);
                    }
                }

                foreach (object item in e.AddedItems)
                {
                    if (this.Region.Views.Contains(item) && !this.Region.ActiveViews.Contains(item))
                    {
                        this.Region.Activate(item, null);
                    }
                }
            }
            finally
            {
                this._updatingActiveViewsInHostControlSelectionChanged = false;
            }
        }
    }
}
