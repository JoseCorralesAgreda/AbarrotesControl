using AbarrotesControl.Core.DTOs;
using AbarrotesControl.Core.Entities;

namespace AbarrotesControl.Services.Interfaces;

public interface IStockService
{
    Task<List<StockDto>> GetAllAsync(int? categoriaId = null, string? buscar = null, bool soloAlertas = false);
    Task<StockDto> GetByIdAsync(int productoId);
    Task<ResumenAlertasDto> GetAlertasAsync(string? tipo = null);
    List<AlertaStockDto> CalcularAlertas(Producto producto, DateOnly hoy);
}
