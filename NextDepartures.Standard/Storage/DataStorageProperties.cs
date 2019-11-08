using NextDepartures.Standard.Storage.Attributes;
using System.Reflection;

namespace NextDepartures.Standard.Storage
{
    public class DataStorageProperties
    {
        public DataStorageProperties()
        {
            this.DoesSupportParallelPreload = false;
        }

        public DataStorageProperties(IDataStorage dataStorage) : this()
        {
            this.DoesSupportParallelPreload = dataStorage.GetType().GetCustomAttribute<SupportsParallelPreloadAttribute>() != null;
        }

        public bool DoesSupportParallelPreload { get; set; }
    }
}
