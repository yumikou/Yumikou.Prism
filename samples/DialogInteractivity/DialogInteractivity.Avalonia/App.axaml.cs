using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DialogInteractivity.ViewModel;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Globalization;

namespace DialogInteractivity.Avalonia
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

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialogWindow<DialogWindow>();
            containerRegistry.RegisterDialog<MyDialogView>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewName = viewType!.FullName!.Replace("DialogInteractivity.Avalonia", "");
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
                        "DialogInteractivity.ViewModel", vmName, //命名空间
                        "DialogInteractivity.ViewModel"); //程序集名称
                return Type.GetType(vmTypeName);
            });
            //ViewModelLocationProvider.Register<MainWindow>(() => Container.Resolve<MainWindowViewModel>());
        }
    }
}