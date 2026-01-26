namespace ModularTemplate.Modules.Sales.Application.Features;

/// <summary>
/// Configuration paths for Sales module feature flags.
/// These flags are defined in modules.sales.json under Sales:Features.
/// </summary>
public static class SalesFeatures
{
    /// <summary>
    /// Configuration path for the Catalog V2 Pagination feature flag.
    /// Controls whether the V2 paginated catalog endpoint is available.
    /// </summary>
    public const string CatalogV2Pagination = "Sales:Features:CatalogV2Pagination";

    /// <summary>
    /// Configuration path for the Bulk Create Products feature flag.
    /// Controls whether bulk product creation operations are enabled.
    /// </summary>
    public const string BulkCreateProducts = "Sales:Features:BulkCreateProducts";
}
