namespace AbarrotesControl.Core.QueryFilters;

public class StockQueryFilter
{
    public int? CategoriaId { get; set; }
    public string? Buscar { get; set; }
    public bool SoloAlertas { get; set; }
}
