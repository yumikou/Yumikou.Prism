using Avalonia.Controls;

namespace DialogInteractivity.Avalonia
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.vdt.Click += Vdt_Click;
        }

        private void Vdt_Click(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            (new MainWindow2()).Show();
        }
    }
}