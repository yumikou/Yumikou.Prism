using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace System
{
    public static class ArrayEx
    {
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static T[] Empty<T>()
        {
            return EmptyArrayEx<T>.Value;
        }
    }

    internal static class EmptyArrayEx<T>
    {
        public static readonly T[] Value = new T[0];
    }
}
