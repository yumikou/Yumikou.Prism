# PrismLibrary

基于 prism 8.1.97 和 prism.avalonia 8.1.97.11000

nuget package:
`Yumikou.Prism.Core`

`Yumikou.Prism.Wpf`
`Yumikou.Prism.DryIoc.Wpf`

`Yumikou.Prism.Avalonia`
`Yumikou.Prism.DryIoc.Avalonia`

## TODO:
### 嵌套Region的默认行为：
- Region所在的view销毁时，自动销毁当前Region关联的所有view，使用bool附加属性控制是否启用，默认为true
- 自动更新子Region的IsActive，使用bool附加属性控制是否启用，默认为false