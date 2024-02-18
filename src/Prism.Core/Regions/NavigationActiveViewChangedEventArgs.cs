using System;
using System.Collections.Generic;
using System.Text;

namespace Prism.Regions
{
    public class NavigationActiveViewChangedEventArgs : EventArgs
    {
        public NavigationType NavigationType { get; set; }

        public object ActiveView { get; set; }

        public NavigationActiveViewChangedEventArgs(object activeView, NavigationType navigationType)
        {
            ActiveView = activeView;
            NavigationType = navigationType;
        }
    }
}
