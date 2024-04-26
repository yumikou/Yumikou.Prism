using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Prism.Services.Dialogs;
using Splat;

namespace VirtualDialog.Avalonia;

public partial class MainView : UserControl
{
    private VirtualDialogWindow vdw;
    public MainView()
    {
        InitializeComponent();

        this.tt.Click += Tt_Click;
    }

    VirtualDialogWindow vd;

    private void Tt_Click(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
    {
        vd ??= new VirtualDialogWindow();
        vd.Owner = this;
        vd.Open();
    }
}