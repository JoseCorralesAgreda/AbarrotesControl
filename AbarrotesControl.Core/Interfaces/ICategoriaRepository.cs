using AbarrotesControl.Core.Entities;

namespace AbarrotesControl.Core.Interfaces;

public interface ICategoriaRepository
{
    Task<List<Categoria>> GetAllAsync();
    Task<Categoria?> GetByIdAsync(int id);
}
