using System;
using Template.DAL;

namespace Template.Tests.Common
{
    public class CommandTestBase : IDisposable
    {
        protected readonly ApplicationDbContext Context;

        public CommandTestBase()
        {
            this.Context = DbContextFactory.Create();
        }

        public void Dispose()
        {
            DbContextFactory.Destroy(this.Context);
        }
    }
}