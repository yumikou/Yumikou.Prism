using Avalonia.Animation;
using Avalonia.Controls;
using AvaloniaInside.Shell.Platform.Android;
using AvaloniaInside.Shell.Platform.Ios;
using AvaloniaInside.Shell.Platform.Windows;
using Prism.Regions;
using System;

namespace StackNavigation.Avalonia
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            this.tcc.PageTransition = DefaultIosPageSlide.Instance;
        }
    }
}