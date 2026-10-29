using Microsoft.EntityFrameworkCore;
using AbarrotesControl.Core.Entities;
using AbarrotesControl.Core.Interfaces;
using AbarrotesControl.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbarrotesControl.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T>
        where T : BaseEntity
    {
        private readonly AbarrotesControlContext _context; //Base de datos
        private readonly DbSet<T> _entities; //Tablas

        public BaseRepository(AbarrotesControlContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _entities.ToListAsync();
        }

        public async Task<T?> GetById(int id)
        {
            return await _entities.FindAsync(id);
        }
        public async Task Add(T entity)
        {
            _entities.Add(entity); //Insert
            await _context.SaveChangesAsync(); //Commit
        }

        public async Task Update(T entity)
        {
            _entities.Update(entity);
            await _context.SaveChangesAsync(); //Commit
        }

        public async Task Delete(int id)
        {
            T entity = await GetById(id) ?? throw new InvalidOperationException("El registro no existe.");
            _entities.Remove(entity);
            await _context.SaveChangesAsync(); //Commit
        }
    }
}
