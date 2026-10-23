using AbarrotesControl.Core.Entities;

namespace AbarrotesControl.Core.Interfaces;

public interface IEntradaRepository
{
    Task<EntradaMercaderia?> GetByIdAsync(int id);
    Task<EntradaMercaderia> InsertAsync(EntradaMercaderia entrada);
}
