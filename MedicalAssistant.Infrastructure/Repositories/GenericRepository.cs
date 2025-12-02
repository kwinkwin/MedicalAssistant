using MedicalAssistant.Domain.Entities;
using MedicalAssistant.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAssistant.Infrastructure.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly MedicalAssistantDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(MedicalAssistantDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet;

            if (predicate != null)
                query = query.Where(predicate);

            if (include != null)
                query = include(query);

            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(object id, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            if (include == null)
            {
                return await _dbSet.FindAsync(id); // dùng FindAsync nếu không cần include
            }

            // Nếu cần include, phải query bằng lambda
            IQueryable<T> query = include(_dbSet);

            var keyProperty = _context.Model.FindEntityType(typeof(T))!
                .FindPrimaryKey()!
                .Properties.First();

            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.Property(parameter, keyProperty.Name);
            var idValue = Expression.Constant(id);
            var equal = Expression.Equal(property, Expression.Convert(idValue, property.Type));
            var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

            return await query.FirstOrDefaultAsync(lambda);
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet.Where(predicate);

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            IQueryable<T> query = _dbSet;

            if (predicate != null)
                query = query.Where(predicate);

            return await query.CountAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Delete(T entity) => _dbSet.Remove(entity);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public IQueryable<T> GetQueryable(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet;

            if (predicate != null)
                query = query.Where(predicate);

            if (include != null)
                query = include(query);

            return query;
        }
    }
}
