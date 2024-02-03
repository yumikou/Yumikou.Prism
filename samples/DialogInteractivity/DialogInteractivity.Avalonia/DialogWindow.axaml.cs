using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Prism.Services.Dialogs;

namespace DialogInteractivity.Avalonia;

public partial class DialogWindow : Window, IDialogWindow
{
    public DialogWindow()
    {
        InitializeComponent();
    }

    public IDialogResult Result { get; set; } = new DialogResult(ButtonResult.None);
}