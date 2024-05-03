using System;
using System.Collections.Generic;
using System.Text;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Prism.Commands;

namespace VirtualDialog.ViewModel
{
    public class ConfirmDialogViewModel : IDialogAware
    {
        public DelegateCommand OkCommand { get; set; }

        public DelegateCommand CancelCommand { get; set; }

        public string Title => "Confirm";

        public event Action<IDialogResult> RequestClose;

        public ConfirmDialogViewModel()
        {
            OkCommand = new DelegateCommand(OnOk);
            CancelCommand = new DelegateCommand(OnCancel);
        }

        private void OnOk()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        private void OnCancel()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        public bool CanCloseDialog(IDialogResult dialogResult)
        {
            return true;
        }

        public void OnDialogClosed(IDialogResult dialogResult)
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }
    }
}
