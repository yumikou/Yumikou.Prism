using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NestedRegion.ViewModel
{
    public class ViewModelHeaderBase : BindableBase, IRequestCreateAware
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public virtual void OnCreate(RequestCreateContext requestCreateContext)
        {
            if (requestCreateContext.Parameters.TryGetValue("Title", out string title))
            { 
                Title = title;
            }
        }
    }
}
