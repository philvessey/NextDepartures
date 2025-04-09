using System;
using JetBrains.Annotations;

namespace NextDepartures.Standard.Exceptions;

[Serializable]
public class StopException : Exception
{
    [UsedImplicitly]
    public StopException() { }
    
    [UsedImplicitly]
    public StopException(string message) : base(message: message) { }
    
    [UsedImplicitly]
    public StopException(string message, Exception innerException) : base(message: message, innerException: innerException) { }
}