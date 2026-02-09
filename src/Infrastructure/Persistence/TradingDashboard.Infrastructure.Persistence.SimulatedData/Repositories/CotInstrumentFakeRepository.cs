using System.Linq.Expressions;
using System.Text.Json;

namespace TradingDashboard.Infrastructure.Persistence.SimulatedData.Repositories;

public sealed class CotInstrumentFakeRepository : ICotInstrumentRepository
{
    private readonly List<CotInstrument> _items;

    public CotInstrumentFakeRepository()
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "Data", "cot-instruments.json"),
            Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..", "..", "..",
                "src", "Infrastructure", "Persistence", "TradingDashboard.Infrastructure.Persistence.SimulatedData",
                "Data", "cot-instruments.json"))
        };

        var dataPath = candidates.FirstOrDefault(File.Exists);

        if (string.IsNullOrWhiteSpace(dataPath))
        {
            _items = [];
            return;
        }

        using var stream = File.OpenRead(dataPath);
        using var document = JsonDocument.Parse(stream);

        _items = document.RootElement
            .GetProperty("instruments")
            .EnumerateArray()
            .Select(item =>
            {
                var idText = item.TryGetProperty("id", out var idProperty)
                    ? idProperty.GetString()
                    : null;
                var id = Guid.TryParse(idText, out var parsedId) ? parsedId : Guid.NewGuid();

                return new CotInstrument(
                    id: id,
                    instrumentId: item.GetProperty("instrumentId").GetString() ?? string.Empty,
                    instrumentSlug: item.GetProperty("instrumentSlug").GetString() ?? string.Empty,
                    instrumentName: item.GetProperty("instrumentName").GetString() ?? string.Empty);
            })
            .ToList();
    }

    public CotInstrument? GetBySlug(string instrumentSlug)
    {
        if (string.IsNullOrWhiteSpace(instrumentSlug))
        {
            throw new ArgumentException("Instrument slug cannot be empty.", nameof(instrumentSlug));
        }

        return _items.FirstOrDefault(x =>
            string.Equals(x.InstrumentSlug, instrumentSlug, StringComparison.OrdinalIgnoreCase));
    }

    public void Create(CotInstrument entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _items.Add(entity);
    }

    public CotInstrument Get(Guid id)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        return item ?? throw new KeyNotFoundException($"{nameof(CotInstrument)} with id '{id}' was not found.");
    }

    public IList<CotInstrument> GetList()
    {
        return _items.ToList();
    }

    public bool Exists(Expression<Func<CotInstrument, bool>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        return _items.AsQueryable().Any(expression);
    }

    public void SaveChanges()
    {
        // No-op in fake implementation.
    }
}
