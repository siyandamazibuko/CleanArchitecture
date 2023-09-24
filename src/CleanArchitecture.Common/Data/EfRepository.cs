using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Common.Data
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public EfRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public IQueryable<T> Table => _dbSet;

        #region IRepository<T> Methods

        Task<int> IRepository<T>.CountAsync(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.CountAsync(predicate);
        }

        IQueryable<T> IRepository<T>.Fetch(Expression<Func<T, bool>> predicate)
        {
            return Fetch(predicate);
        }

        IQueryable<T> IRepository<T>.Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order)
        {
            return Fetch(predicate, order);
        }

        IQueryable<T> IRepository<T>.Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int skip, int count)
        {
            return Fetch(predicate, order, skip, count);
        }

        #endregion  

        #region Public Methods

        public virtual Task<List<T>> GetAllAsync()
        {
            return _dbSet.ToListAsync();
        }

        public virtual void Create(T entity)
        {
            _dbSet.Add(entity);
        }

        public virtual Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.AnyAsync(predicate);
        }

        public virtual void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(Guid id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null) _dbSet.Remove(entity);
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void Delete(Expression<Func<T, bool>> predicate)
        {
            var objects = _dbSet.Where(predicate).AsEnumerable();
            foreach (var obj in objects)
                _dbSet.Remove(obj);
        }

        public virtual T Get(Guid id)
        {
            return _dbSet.Find(id);
        }

        public Task ReloadAsync(T entity)
        {
            return _dbContext.Entry(entity).ReloadAsync();
        }

        public virtual Task<T> GetAsync(Guid id)
        {
            return _dbSet.FindAsync(id).AsTask();
        }

        public virtual Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual List<T> GetAll()
        {
            return _dbSet.ToList();
        }

        #endregion

        #region Private Methods                             

        private IQueryable<T> Fetch(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        private IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order)
        {
            var orderable = new Orderable<T>(Fetch(predicate));
            order(orderable);
            return orderable.Queryable;
        }

        private IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int skip, int count)
        {
            return Fetch(predicate, order).Skip(skip).Take(count);
        }

        #endregion
    }
}
