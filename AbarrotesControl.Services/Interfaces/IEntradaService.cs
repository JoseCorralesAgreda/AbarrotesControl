using AbarrotesControl.Core.Entities;

namespace AbarrotesControl.Services.Interfaces;

public interface IEntradaService
{
    Task<EntradaMercaderia> GetByIdAsync(int id);
    Task<EntradaMercaderia> InsertAsync(EntradaMercaderia entrada);
}
