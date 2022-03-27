using System;
using Microsoft.EntityFrameworkCore;
using Template.DAL;
using Template.DAL.Models.Entities;

namespace Template.Tests.Common
{
    public static class DbContextFactory
    {
        public static ApplicationDbContext Create()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var user1 = new User() {Id = "0001", UserName = "User 1"};
            var user2 = new User() {Id = "0002", UserName = "User 2"};

            var context = new ApplicationDbContext(options);

            context.Database.EnsureCreated();

            context.Users.AddRange(new[]
            {
                user1,
                user2,
            });

            context.SaveChanges();

            return context;
        }

        public static void Destroy(ApplicationDbContext context)
        {
            context.Database.EnsureDeleted();

            context.Dispose();
        }
    }
}