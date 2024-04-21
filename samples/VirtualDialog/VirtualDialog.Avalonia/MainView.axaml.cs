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

        this.Loaded += MainView_Loaded;
        
    }

    private void MainView_Loaded(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
    {
        VirtualDialogWindow vd = new VirtualDialogWindow();
        var ol = OverlayLayer.GetOverlayLayer(Window.GetTopLevel(this));

        VirtualDialogOverlayLayer vdol = new VirtualDialogOverlayLayer();
        vdol.Content = vd;

        ol.Children.Add(vdol);
    }
}