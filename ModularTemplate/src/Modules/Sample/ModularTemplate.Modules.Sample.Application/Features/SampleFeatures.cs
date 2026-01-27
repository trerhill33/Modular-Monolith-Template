namespace ModularTemplate.Modules.Sample.Application.Features;

/// <summary>
/// Configuration paths for Sample module feature flags.
/// These flags are defined in modules.sample.json under Sample:Features.
/// </summary>
public static class SampleFeatures
{
    /// <summary>
    /// Configuration path for the Catalog V2 Pagination feature flag.
    /// Controls whether the V2 paginated catalog endpoint is available.
    /// </summary>
    public const string CatalogV2Pagination = "Sample:Features:CatalogV2Pagination";

    /// <summary>
    /// Configuration path for the Bulk Create Products feature flag.
    /// Controls whether bulk product creation operations are enabled.
    /// </summary>
    public const string BulkCreateProducts = "Sample:Features:BulkCreateProducts";
}
