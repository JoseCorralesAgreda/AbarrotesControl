using AbarrotesControl.Core.Entities;
using AbarrotesControl.Core.Interfaces;
using AbarrotesControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AbarrotesControl.Infrastructure.Repositories;

public class CategoriaRepository(AbarrotesControlContext context) : ICategoriaRepository
{
    public Task<List<Categoria>> GetAllAsync() =>
        context.Categorias.AsNoTracking().ToListAsync();

    public Task<Categoria?> GetByIdAsync(int id) =>
        context.Categorias.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
}
