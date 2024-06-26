﻿using Avalonia.Controls;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Prism.Services.Dialogs;

namespace Prism
{
    internal static class PrismInitializationExtensions
    {
        internal static void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                return ContainerLocator.Container.Resolve(type);
            });
        }

        internal static void RegisterRequiredTypes(this IContainerRegistry containerRegistry, IModuleCatalog moduleCatalog)
        {
            containerRegistry.RegisterInstance(moduleCatalog);
            containerRegistry.RegisterSingleton<IDialogService, DialogService>();
            containerRegistry.RegisterSingleton<IVirtualDialogService, VirtualDialogService>();
            containerRegistry.RegisterSingleton<IModuleInitializer, ModuleInitializer>();
            containerRegistry.RegisterSingleton<IModuleManager, ModuleManager>();
            containerRegistry.RegisterSingleton<RegionAdapterMappings>();
            containerRegistry.RegisterSingleton<IRegionManager, RegionManager>();
            containerRegistry.RegisterSingleton<IRegionNavigationContentLoader, RegionNavigationContentLoader>();
            containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();
            containerRegistry.RegisterSingleton<IRegionViewRegistry, RegionViewRegistry>();
            containerRegistry.RegisterSingleton<IRegionBehaviorFactory, RegionBehaviorFactory>();
            containerRegistry.Register<IRegionNavigationJournalEntry, RegionNavigationJournalEntry>();
            containerRegistry.Register<IRegionNavigationJournal, RegionNavigationJournal>();
            containerRegistry.Register<IRegionNavigationService, RegionNavigationService>();
            containerRegistry.Register<IRegionRequestCreateService, RegionRequestCreateService>();
            containerRegistry.Register<IDialogWindow, DialogWindow>(); //default dialog host
            containerRegistry.Register<IVirtualDialogWindow, VirtualDialogWindow>(); //default virtual dialog host
        }

        internal static void RegisterDefaultRegionBehaviors(this IRegionBehaviorFactory regionBehaviors)
        {
            //// Avalonia to WPF Equivilant: BindRegionContextToAvaloniaObjectBehavior == BindRegionContextToDependencyObjectBehavior
            regionBehaviors.AddIfMissing<BindRegionContextToAvaloniaObjectBehavior>(BindRegionContextToAvaloniaObjectBehavior.BehaviorKey);
            regionBehaviors.AddIfMissing<RegionActiveAwareBehavior>(RegionActiveAwareBehavior.BehaviorKey);
            regionBehaviors.AddIfMissing<SyncRegionContextWithHostBehavior>(SyncRegionContextWithHostBehavior.BehaviorKey);
            regionBehaviors.AddIfMissing<RegionManagerRegistrationBehavior>(RegionManagerRegistrationBehavior.BehaviorKey);
            regionBehaviors.AddIfMissing<RegionMemberLifetimeBehavior>(RegionMemberLifetimeBehavior.BehaviorKey);
            regionBehaviors.AddIfMissing<ClearChildViewsRegionBehavior>(ClearChildViewsRegionBehavior.BehaviorKey);
            regionBehaviors.AddIfMissing<AutoPopulateRegionBehavior>(AutoPopulateRegionBehavior.BehaviorKey);
            regionBehaviors.AddIfMissing<DestructibleRegionBehavior>(DestructibleRegionBehavior.BehaviorKey);
        }

        internal static void RegisterDefaultRegionAdapterMappings(this RegionAdapterMappings regionAdapterMappings)
        {
            //// regionAdapterMappings.RegisterMapping<Selector, SelectorRegionAdapter>();
            regionAdapterMappings.RegisterMapping<ItemsControl, ItemsControlRegionAdapter>();
            regionAdapterMappings.RegisterMapping<ContentControl, ContentControlRegionAdapter>();
            regionAdapterMappings.RegisterMapping<TransitioningContentControl, TransitioningContentControlRegionAdapter>();
            regionAdapterMappings.RegisterMapping<TabControl, TabControlRegionAdapter>();
        }

        internal static void RunModuleManager(IContainerProvider containerProvider)
        {
            IModuleManager manager = containerProvider.Resolve<IModuleManager>();
            manager.Run();
        }
    }
}
