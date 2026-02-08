using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests
{
    public class InMemoryDbContext : IDisposable
    {
        public ApplicationDbContext Context { get; private set; } = null!;

        public InMemoryDbContext()
        {
            Recreate();
        }

        public void Dispose()
        {
            Context.Dispose();
        }

        public void Recreate()
        {
            CreateDatabase();
            SeedData();
        }

        public void CreateDatabase()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            Context = new ApplicationDbContext(options);
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
        }

        public void SeedData()
        {
            for (int i = 1; i <= 5; i++)
            {
                var user = new User
                {
                    FirstName = $"FirstName{i}",
                    LastName = $"LastName{i}",
                    Email = $"email{i}@domain.com",
                    PhoneNumber = $"+000000000{i}",
                };

                var contacts = Enumerable.Range(1, 5).Select(j => new Contact
                {
                    FirstName = $"FirstName{j}",
                    LastName = $"LastName{j}",
                    Email = $"email{j}@domain.com",
                    PhoneNumber = $"+000000000{j}",
                    UserId = user.Id,
                }).ToList();

                foreach (var contact in contacts)
                {
                    user.Contacts.Add(contact);
                }

                Context.Users.Add(user);
            }

            Context.SaveChanges();
        }
    }
}
