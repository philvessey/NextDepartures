using System;

namespace NextDepartures.Standard.Storage.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class SupportsParallelPreloadAttribute : Attribute
    {
    }
}
