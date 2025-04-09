using System;
using JetBrains.Annotations;

namespace NextDepartures.Standard.Exceptions;

[Serializable]
public class ServiceException : Exception
{
    [UsedImplicitly]
    public ServiceException() { }
    
    [UsedImplicitly]
    public ServiceException(string message) : base(message: message) { }
    
    [UsedImplicitly]
    public ServiceException(string message, Exception innerException) : base(message: message, innerException: innerException) { }
}