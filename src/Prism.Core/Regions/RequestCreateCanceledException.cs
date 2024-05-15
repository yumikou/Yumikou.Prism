using System;
using System.Collections.Generic;
using System.Text;

namespace Prism.Regions
{
    public class RequestCreateCanceledException : Exception
    {
        public RequestCreateCanceledException() { }

        public RequestCreateCanceledException(string message) : base(message) { }
    }
}
