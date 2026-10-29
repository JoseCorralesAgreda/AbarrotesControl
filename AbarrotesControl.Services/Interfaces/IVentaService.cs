using AbarrotesControl.Core.DTOs;
using AbarrotesControl.Core.Entities;

namespace AbarrotesControl.Services.Interfaces;

public interface IVentaService
{
    Task<VentaDto> InsertAsync(Venta venta);
    Task<VentaDto> GetByIdAsync(int id);
    Task<List<VentaDto>> GetAllAsync();
}
