using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CleanArchitecture.Common.Data
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private bool _disposed;

        public EfUnitOfWork(DbContext dbContext)
        {
            DbContext = dbContext;
        }

        public DbContext DbContext { get; }

        public void Commit()
        {
            try
            {
                var numberOfChanges = DbContext.SaveChanges();
                Log.Debug("DBContext changed {Count} entities", numberOfChanges);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public async Task CommitAsync()
        {
            try
            {
                var numberOfChanges = await DbContext.SaveChangesAsync();
                Log.Debug("DBContext changed {Count} entities", numberOfChanges);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public void ClearChangeTracker()
        {
            DbContext.ChangeTracker.Clear();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                DbContext.Dispose();
            }
            _disposed = true;
        }

        private static void HandleException(Exception ex)
        {
            var exception = ex.GetBaseException();
            Log.Error(exception, exception.Message);
            throw exception;
        }

        #endregion
    }
}
