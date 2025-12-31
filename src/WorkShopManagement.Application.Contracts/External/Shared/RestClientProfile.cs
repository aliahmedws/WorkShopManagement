using System;
using System.Collections.Generic;

namespace WorkShopManagement.External.Shared;

public sealed class RestClientProfile
{
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(30);

    // Optional defaults (keep minimal)
    public IDictionary<string, string> DefaultQueryParams { get; init; } = new Dictionary<string, string>();
    public IDictionary<string, string> DefaultHeaders { get; init; } = new Dictionary<string, string>();
}
