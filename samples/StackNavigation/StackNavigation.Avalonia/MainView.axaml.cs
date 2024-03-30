using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell.Platform.Ios;

namespace StackNavigation.Avalonia;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        this.tcc.PageTransition = DefaultIosPageSlide.Instance;
    }
}