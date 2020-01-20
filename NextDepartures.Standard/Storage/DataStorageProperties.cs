﻿using NextDepartures.Standard.Storage.Attributes;
using System.Reflection;

namespace NextDepartures.Standard.Storage
{
    public class DataStorageProperties
    {
        public bool DoesSupportParallelPreload { get; set; }

        public DataStorageProperties()
        {
            DoesSupportParallelPreload = false;
        }

        public DataStorageProperties(IDataStorage dataStorage) : this()
        {
            DoesSupportParallelPreload = dataStorage.GetType().GetCustomAttribute<SupportsParallelPreloadAttribute>() != null;
        }
    }
}