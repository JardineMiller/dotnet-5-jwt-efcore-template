using System;
using System.Threading;
using System.Threading.Tasks;
using DevTracker.DAL.Models.Base;
using DevTracker.DAL.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevTracker.DAL
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            base.OnModelCreating(builder);
        }

        #region Audit Info

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ApplyAuditInformation();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = new CancellationToken())
        {
            ApplyAuditInformation();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void ApplyAuditInformation()
        {
            var auditEntries = this.ChangeTracker.Entries<IAuditableEntity>();

            foreach (var entityEntry in auditEntries)
            {
                switch (entityEntry.State)
                {
                    case EntityState.Added:
                        entityEntry.Entity.CreatedOn = DateTimeOffset.UtcNow;
                        break;

                    case EntityState.Modified:
                        entityEntry.Entity.ModifiedOn = DateTimeOffset.UtcNow;
                        break;

                    case EntityState.Deleted:
                        if (entityEntry.Entity is ISoftDeletable deletableEntity)
                        {
                            deletableEntity.DeletedOn = DateTimeOffset.UtcNow;
                            deletableEntity.IsDeleted = true;

                            entityEntry.State = EntityState.Modified;
                        }

                        break;
                }
            }
        }

        #endregion
    }


}
