using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using VirtualDialog.Avalonia.ViewModels;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using System.Globalization;
using System;

namespace VirtualDialog.Avalonia
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
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
                return Container.Resolve<MainWindow>();
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewLifetime)
                return Container.Resolve<MainView>();
            return null;
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

        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewName = viewType!.FullName!.Replace("VirtualDialog.Avalonia.Views", "");
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
                        "VirtualDialog.ViewModel", vmName, //命名空间
                        "VirtualDialog.ViewModel"); //程序集名称
                return Type.GetType(vmTypeName);
            });

            ViewModelLocationProvider.Register<MainView>(() => Container.Resolve<MainViewModel>());
        }
    }
}