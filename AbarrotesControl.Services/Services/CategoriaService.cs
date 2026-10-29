using AbarrotesControl.Core.Entities;
using AbarrotesControl.Core.Interfaces;
using AbarrotesControl.Services.Interfaces;

namespace AbarrotesControl.Services.Services;

public class CategoriaService(IBaseRepository<Categoria> repository) : ICategoriaService
{
    public async Task<IEnumerable<Categoria>> GetAllAsync() => await repository.GetAll();
}
