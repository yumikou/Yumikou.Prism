using Avalonia.Animation;
using Avalonia.Controls;
using Prism.Regions;
using System;

namespace StackNavigation.Avalonia
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.tcc.PageTransition = new PageSlide(TimeSpan.FromMilliseconds(200), PageSlide.SlideAxis.Vertical);
        }
    }
}