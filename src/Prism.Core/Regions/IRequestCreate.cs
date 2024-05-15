using System;
using System.Collections.Generic;
using System.Text;

namespace Prism.Regions
{
    public interface IRequestCreate
    {
        RequestCreateResult RequestCreate(Uri target, bool cancelIfExists);

        RequestCreateResult RequestCreate(Uri target, NavigationParameters parameters, bool cancelIfExists);
    }
}
