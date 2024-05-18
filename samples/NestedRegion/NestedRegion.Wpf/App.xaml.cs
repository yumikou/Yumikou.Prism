using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;
using NestedRegion.ViewModel;
using NestedRegion.Wpf.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;

namespace NestedRegion.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<AView>();
            containerRegistry.RegisterForNavigation<BView>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewName = viewType!.FullName!.Replace("NestedRegion.Wpf.Views", "");
                var vmName = "";
                if (viewName.EndsWith("View"))
                {
                    vmName = viewName.Substring(0, viewName.Length - 4) + "ViewModel";
                }
                else
                {
                    vmName = viewName + "ViewModel";
                }
                var vmTypeName = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}{1}, {2}",
                        "NestedRegion.ViewModel", vmName, //命名空间
                        "NestedRegion.ViewModel"); //程序集名称
                return Type.GetType(vmTypeName);
            });

            ViewModelLocationProvider.Register<MainWindow>(() => Container.Resolve<MainWindowViewModel>());
        }
    }
}
