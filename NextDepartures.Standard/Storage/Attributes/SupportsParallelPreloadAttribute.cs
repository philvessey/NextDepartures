using System;

namespace NextDepartures.Standard.Storage.Attributes;

[AttributeUsage(validOn: AttributeTargets.Class)]
public abstract class SupportsParallelPreloadAttribute : Attribute;