using System;
using JetBrains.Annotations;

namespace NextDepartures.Standard.Exceptions;

[Serializable]
public class AgencyException : Exception
{
    [UsedImplicitly]
    public AgencyException() { }
    
    [UsedImplicitly]
    public AgencyException(string message) : base(message: message) { }
    
    [UsedImplicitly]
    public AgencyException(string message, Exception innerException) : base(message: message, innerException: innerException) { }
}