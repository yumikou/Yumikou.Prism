using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Prism.Regions
{
    public class NavigationNotifyCollectionChangedEventArgs : NotifyCollectionChangedEventArgs
    {
        public NavigationType? NavigationType { get; set; }

        public NavigationNotifyCollectionChangedEventArgs(NavigationType? navigationType, NotifyCollectionChangedAction action) : base(action)
        {
            NavigationType = navigationType;
        }

        public NavigationNotifyCollectionChangedEventArgs(NavigationType? navigationType, NotifyCollectionChangedAction action, IList changedItems) : base(action, changedItems)
        {
            NavigationType = navigationType;
        }

        public NavigationNotifyCollectionChangedEventArgs(NavigationType? navigationType, NotifyCollectionChangedAction action, object changedItem) : base(action, changedItem)
        {
            NavigationType = navigationType;
        }

        public NavigationNotifyCollectionChangedEventArgs(NavigationType? navigationType, NotifyCollectionChangedAction action, IList newItems, IList oldItems) : base(action, newItems, oldItems)
        {
            NavigationType = navigationType;
        }

        public NavigationNotifyCollectionChangedEventArgs(NavigationType? navigationType, NotifyCollectionChangedAction action, IList changedItems, int startingIndex) : base(action, changedItems, startingIndex)
        {
            NavigationType = navigationType;
        }

        public NavigationNotifyCollectionChangedEventArgs(NavigationType? navigationType, NotifyCollectionChangedAction action, object changedItem, int index) : base(action, changedItem, index)
        {
            NavigationType = navigationType;
        }

        public NavigationNotifyCollectionChangedEventArgs(NavigationType? navigationType, NotifyCollectionChangedAction action, object newItem, object oldItem) : base(action, newItem, oldItem)
        {
            NavigationType = navigationType;
        }

        public NavigationNotifyCollectionChangedEventArgs(NavigationType? navigationType, NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex) : base(action, newItems, oldItems, startingIndex)
        {
            NavigationType = navigationType;
        }

        public NavigationNotifyCollectionChangedEventArgs(NavigationType? navigationType, NotifyCollectionChangedAction action, IList changedItems, int index, int oldIndex) : base(action, changedItems, index, oldIndex)
        {
            NavigationType = navigationType;
        }

        public NavigationNotifyCollectionChangedEventArgs(NavigationType? navigationType, NotifyCollectionChangedAction action, object changedItem, int index, int oldIndex) : base(action, changedItem, index, oldIndex)
        {
            NavigationType = navigationType;
        }

        public NavigationNotifyCollectionChangedEventArgs(NavigationType? navigationType, NotifyCollectionChangedAction action, object newItem, object oldItem, int index) : base(action, newItem, oldItem, index)
        {
            NavigationType = navigationType;
        }
    }
}
