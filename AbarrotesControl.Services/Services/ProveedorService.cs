using AbarrotesControl.Core.Entities;
using AbarrotesControl.Core.Interfaces;
using AbarrotesControl.Services.Interfaces;

namespace AbarrotesControl.Services.Services;

public class ProveedorService(IBaseRepository<Proveedor> repository) : IProveedorService
{
    public async Task<IEnumerable<Proveedor>> GetAllAsync() => (await repository.GetAll()).Where(x => x.Activo).ToList();
}
