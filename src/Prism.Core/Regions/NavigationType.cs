using System;
using System.Collections.Generic;
using System.Text;

namespace Prism.Regions
{
    public enum NavigationType
    {
        /// <summary>
        /// 导航到新的View
        /// </summary>
        Navigate,
        /// <summary>
        /// 首次Navigate，和Navigate是相同的行为，用来区分Region中的第一次导航
        /// </summary>
        Init,
        GoBack,
        GoForward
    }
}
