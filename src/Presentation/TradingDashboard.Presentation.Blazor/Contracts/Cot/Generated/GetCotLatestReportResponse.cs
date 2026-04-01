using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.Cot.Generated;

public sealed class GetCotLatestReportResponse
{
    [JsonPropertyName("categories")]
    public IReadOnlyList<GetCotLatestReportCategoryResponse>? Categories { get; set; }

    [JsonPropertyName("changeInOpenInterestAll")]
    public int? ChangeInOpenInterestAll { get; set; }

    [JsonPropertyName("contractUnits")]
    public string? ContractUnits { get; set; }

    [JsonPropertyName("instrumentCode")]
    public string? InstrumentCode { get; set; }

    [JsonPropertyName("instrumentId")]
    public string? InstrumentId { get; set; }

    [JsonPropertyName("instrumentName")]
    public string? InstrumentName { get; set; }

    [JsonPropertyName("instrumentSlug")]
    public string? InstrumentSlug { get; set; }

    [JsonPropertyName("marketAndExchangeNames")]
    public string? MarketAndExchangeNames { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("openInterestAll")]
    public int? OpenInterestAll { get; set; }

    [JsonPropertyName("reportDate")]
    public DateTime? ReportDate { get; set; }

    [JsonPropertyName("totalTraders")]
    public int? TotalTraders { get; set; }
}
