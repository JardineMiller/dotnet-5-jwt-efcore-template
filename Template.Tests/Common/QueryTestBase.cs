using System;
using Template.DAL;

namespace Template.Tests.Common
{
    public class QueryTestBase : IDisposable
    {
        public ApplicationDbContext Context { get; }

        public QueryTestBase()
        {
            this.Context = DbContextFactory.Create();
        }

        public void Dispose()
        {
            DbContextFactory.Destroy(this.Context);
        }
    }
}