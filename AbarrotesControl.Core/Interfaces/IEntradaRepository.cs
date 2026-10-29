using AbarrotesControl.Core.Entities;

namespace AbarrotesControl.Core.Interfaces;

public interface IEntradaRepository : IBaseRepository<EntradaMercaderia>
{
    Task<EntradaMercaderia?> GetByIdAsync(int id);
    Task<EntradaMercaderia> InsertAsync(EntradaMercaderia entrada);
}
