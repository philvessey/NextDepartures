using JetBrains.Annotations;

namespace NextDepartures.Standard.Models;

[UsedImplicitly]
public enum DayOffsetType
{
    Yesterday = -1,
    Today = 0,
    Tomorrow = 1
}