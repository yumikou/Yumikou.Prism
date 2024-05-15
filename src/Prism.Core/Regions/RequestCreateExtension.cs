using System;
using System.Collections.Generic;
using System.Text;

namespace Prism.Regions
{
    public static class RequestCreateExtension
    {
        public static RequestCreateResult RequestCreate(this IRequestCreate requestCreate, string target, bool cancelIfExists = false)
        {
            return RequestCreate(requestCreate, target, null, cancelIfExists);
        }

        public static RequestCreateResult RequestCreate(this IRequestCreate requestCreate, string target, NavigationParameters navigationParameters, bool cancelIfExists = false)
        {
            if (requestCreate == null)
                throw new ArgumentNullException(nameof(requestCreate));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            return requestCreate.RequestCreate(new Uri(target, UriKind.RelativeOrAbsolute), navigationParameters, cancelIfExists);
        }
    }
}
