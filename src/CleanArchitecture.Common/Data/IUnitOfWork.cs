using System;
using System.Threading.Tasks;

namespace CleanArchitecture.Common.Data
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        Task CommitAsync();
        void ClearChangeTracker();
    }
}
