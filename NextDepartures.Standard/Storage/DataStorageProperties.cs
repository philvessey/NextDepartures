using System.Reflection;
using NextDepartures.Standard.Storage.Attributes;

namespace NextDepartures.Standard.Storage;

public class DataStorageProperties
{
    public bool DoesSupportParallelPreload { get; }
    
    private DataStorageProperties()
    {
        DoesSupportParallelPreload = false;
    }
    
    public DataStorageProperties(IDataStorage dataStorage) : this()
    {
        DoesSupportParallelPreload = dataStorage
            .GetType()
            .GetCustomAttribute<SupportsParallelPreloadAttribute>() is not null;
    }
}