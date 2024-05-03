using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualDialog.ViewModel
{
    public class MyDialogViewModel : BindableBase, IDialogAware
    {
        private IVirtualDialogService _virtualDialogService;
        private bool _isClosingConfirmed = false;

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty<string>(ref _title, value); }
        }

        public event Action<IDialogResult> RequestClose;

        public MyDialogViewModel(IVirtualDialogService virtualDialogService)
        { 
            _virtualDialogService = virtualDialogService;
        }

        public bool CanCloseDialog(IDialogResult dialogResult)
        {
            if (!_isClosingConfirmed)
            {
                ShowConfirmedDialog(dialogResult);
                return false;
            }
            return true;
        }

        private async void ShowConfirmedDialog(IDialogResult dialogResult)
        {
            var result = await _virtualDialogService.ShowDialogAsync("ConfirmDialogView", new DialogParameters());
            if (result.Result == ButtonResult.OK)
            {
                _isClosingConfirmed = true;
                RequestClose?.Invoke(dialogResult);
            }
        }

        public void OnDialogClosed(IDialogResult dialogResult)
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
