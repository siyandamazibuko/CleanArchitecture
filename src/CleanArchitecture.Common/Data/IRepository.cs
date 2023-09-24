using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CleanArchitecture.Common.Data
{
    public interface IRepository<T> where T: class {        
        void Create(T entity);
        void Update(T entity);
        void Delete(Guid id);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> predicate);        
        IQueryable<T> Table { get; }
        List<T> GetAll();
        Task<List<T>> GetAllAsync();
        Task<T> GetAsync(Guid id);
        Task<T> GetAsync(Expression<Func<T, bool>> predicate);                
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> Fetch(Expression<Func<T, bool>> predicate);
        IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order);
        IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int skip, int count);
        T Get(Guid id);
        Task ReloadAsync(T entity);
    }
}
