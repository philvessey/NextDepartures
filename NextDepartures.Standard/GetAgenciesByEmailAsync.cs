﻿using GTFS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDepartures.Standard;

public partial class Feed
{
    /// <summary>
    /// Gets the agencies by the given email.
    /// </summary>
    /// <param name="email">The email. Default is all but can be overridden.</param>
    /// <param name="count">The maximum number of results to return. Default is all (0) but can be overridden.</param>
    /// <returns>A list of agencies.</returns>
    public async Task<List<Agency>> GetAgenciesByEmailAsync(string email = "", int count = 0)
    {
        try
        {
            var agenciesFromStorage = await _dataStorage.GetAgenciesByEmailAsync(email);
            return count > 0 ? agenciesFromStorage.Take(count).ToList() : agenciesFromStorage;
        }
        catch
        {
            return null;
        }
    }
}