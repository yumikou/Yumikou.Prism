using System;
using System.Collections.Generic;
using System.Text;

namespace Prism.Regions
{
    public enum RegionMemberLifetimeType
    {
        /// <summary>
        /// View在Deactivate状态下也不会释放
        /// </summary>
        KeepAlive,
        /// <summary>
        /// 在导航前进时保留，返回时释放
        /// </summary>
        Stack,
        /// <summary>
        /// Deactivate状态下立即释放View
        /// </summary>
        Transient
    }
}
