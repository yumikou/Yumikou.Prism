using System;
using System.Collections.Generic;
using System.Text;

namespace Prism.Regions
{
    public enum NavigationType
    {
        /// <summary>
        /// 导航到新的View, 并激活该View
        /// </summary>
        Navigate,
        GoBack,
        GoForward
    }
}
