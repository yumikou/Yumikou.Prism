using System;
using System.Collections.Generic;
using System.Text;

namespace Prism.Regions
{
    public class ItemMetadataIsActiveChangedEventArgs : EventArgs
    {
        public bool IsActive { get; set;}

        public NavigationType NavigationType { get; set;}

        public ItemMetadataIsActiveChangedEventArgs(bool isActive, NavigationType navigationType)
        { 
            IsActive = isActive;
            NavigationType = navigationType;
        }
    }
}
