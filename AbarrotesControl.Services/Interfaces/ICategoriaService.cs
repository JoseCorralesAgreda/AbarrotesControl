using AbarrotesControl.Core.Entities;

namespace AbarrotesControl.Services.Interfaces;

public interface ICategoriaService
{
    Task<IEnumerable<Categoria>> GetAllAsync();
}
