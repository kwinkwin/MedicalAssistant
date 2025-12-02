using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAssistant.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(object id, Func<IQueryable<T>, IQueryable<T>>? include = null);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IQueryable<T>>? include = null);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveChangesAsync();
        IQueryable<T> GetQueryable(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IQueryable<T>>? include = null);
    }
}
