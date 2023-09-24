 using Microsoft.EntityFrameworkCore;

 namespace CleanArchitecture.Common.Data
{
    public interface IDbContextFactory
    {
        IUnitOfWork Create();
        DbContext CurrentContext { get; }
        void Close();
    }
}
