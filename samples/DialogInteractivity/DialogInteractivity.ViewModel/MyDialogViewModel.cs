using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DialogInteractivity.ViewModel
{
    public class MyDialogViewModel : BindableBase, IDialogAware
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty<string>(ref _title, value); }
        }

        public event Action<IDialogResult> RequestClose;

        public MyDialogViewModel()
        { 
            
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            string title;
            if (parameters.TryGetValue<string>("Title", out title))
            {
                this.Title = title;
            }
        }
    }
}
