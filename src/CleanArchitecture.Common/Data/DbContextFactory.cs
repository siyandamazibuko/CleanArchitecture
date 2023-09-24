using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Common.Data
{
    public class DbContextFactory : IDbContextFactory
    {
        [ThreadStatic]
        private static AsyncLocal<Stack<DbContext>> _localStack;

        private static Stack<DbContext> LocalStack
        {
            get
            {
                if (_localStack?.Value == null)
                {
                    _localStack = new AsyncLocal<Stack<DbContext>> { Value = new Stack<DbContext>() };
                }
                return _localStack.Value;
            }
        }

        private readonly Func<DbContext> _dbContextFactory;

        public DbContextFactory(Func<DbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public DbContext CurrentContext => (LocalStack.Count > 0) ? LocalStack.Peek() : CreateNewDbContext();

        private DbContext CreateNewDbContext()
        {
            var repository = _dbContextFactory();
            LocalStack.Push(repository);
            return repository;
        }

        public IUnitOfWork Create()
        {
            var dbContext = CreateNewDbContext();
            return new EfUnitOfWork(dbContext);
        }

        public void Close()
        {
            CurrentContext.Dispose();
            LocalStack.Pop();
        }
    }
}
