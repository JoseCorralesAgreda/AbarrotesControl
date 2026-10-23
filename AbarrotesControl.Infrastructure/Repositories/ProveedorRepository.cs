using AbarrotesControl.Core.Entities;
using AbarrotesControl.Core.Interfaces;
using AbarrotesControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AbarrotesControl.Infrastructure.Repositories;

public class ProveedorRepository(AbarrotesControlContext context) : IProveedorRepository
{
    public Task<List<Proveedor>> GetAllAsync() =>
        context.Proveedores.AsNoTracking().Where(x => x.Activo).ToListAsync();

    public Task<Proveedor?> GetByIdAsync(int id) =>
        context.Proveedores.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
}
