using System;
using System.Collections.Generic;
using System.Text;

namespace Prism.Regions
{
    public enum PersistInHistoryType
    {
        /// <summary>
        /// 添加到导航历史中
        /// </summary>
        InHistory,
        /// <summary>
        /// 添加到导航历史中，在前进和后退时都跳过该页面
        /// </summary>
        InHistoryAndSkipGoBackGoForward,
        /// <summary>
        /// 添加到导航历史中，在返回时跳过该页面
        /// </summary>
        InHistoryAndSkipGoBack,
        /// <summary>
        /// 不添加到导航历史中（通常在类似TabControl的导航中使用）
        /// 注意：使用堆栈导航（StackNavigation）时，不能设置为NotInHistory，可以使用InHistoryAndSkipGoBackGoForward来模拟不在导航历史中的场景
        /// </summary>
        NotInHistory
    }
}
