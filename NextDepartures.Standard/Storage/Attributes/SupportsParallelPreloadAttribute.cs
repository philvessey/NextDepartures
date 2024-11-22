using System;

namespace NextDepartures.Standard.Storage.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public abstract class SupportsParallelPreloadAttribute : Attribute;