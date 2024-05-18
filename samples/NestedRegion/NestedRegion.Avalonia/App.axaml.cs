using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Globalization;
using Avalonia.Controls;
using NestedRegion.Avalonia.Views;
using NestedRegion.ViewModel;

namespace NestedRegion.Avalonia
{
    public partial class App : PrismApplication
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            base.Initialize();
        }

        protected override AvaloniaObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override DryIoc.Rules CreateContainerRules()
        {
#if _Aot_
            return base.CreateContainerRules().WithUseInterpretation();
#else
            return base.CreateContainerRules();
#endif
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<AView>();
            containerRegistry.RegisterForNavigation<BView>();
            containerRegistry.RegisterForNavigation<BN1View>();
            containerRegistry.RegisterForNavigation<BN2View>();
            containerRegistry.RegisterForNavigation<BN3View>();
            containerRegistry.RegisterForNavigation<BN1N1View>();
            containerRegistry.RegisterForNavigation<BN1N2View>();

            containerRegistry.RegisterDialog<DialogView>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewName = viewType!.FullName!.Replace("NestedRegion.Avalonia.Views", "");
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