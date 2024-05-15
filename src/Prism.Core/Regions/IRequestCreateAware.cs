using System;
using System.Collections.Generic;
using System.Text;

namespace Prism.Regions
{
    public interface IRequestCreateAware
    {
        void OnCreated(RequestCreateContext requestCreateContext);
    }
}
