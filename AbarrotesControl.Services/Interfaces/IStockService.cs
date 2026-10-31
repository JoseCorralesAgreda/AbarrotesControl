using AbarrotesControl.Core.QueryFilters;
using AbarrotesControl.Core.DTOs;
using AbarrotesControl.Core.Entities;

namespace AbarrotesControl.Services.Interfaces;

public interface IStockService
{
    Task<List<StockDto>> GetAllAsync(StockQueryFilter filtro);
    Task<StockDto> GetByIdAsync(int productoId);
    Task<ResumenAlertasDto> GetAlertasAsync(AlertaQueryFilter filtro);
    List<AlertaStockDto> CalcularAlertas(Producto producto, DateOnly hoy);
}
