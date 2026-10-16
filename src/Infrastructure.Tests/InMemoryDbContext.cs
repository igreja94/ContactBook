using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests
{
    public class InMemoryDbContext : IDisposable
    {
        private readonly string databaseName;

        public InMemoryDbContext()
        {
            databaseName = Guid.NewGuid().ToString();
            Recreate();
        }

        public ApplicationDbContext Context { get; private set; } = null!;

        public void Dispose()
        {
            Context.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Recreate()
        {
            CreateDatabase();
            SeedData();
        }

        public void CreateDatabase()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName).Options;
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

                Context.Users.Add(user);

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
                    Context.Contacts.Add(contact);
                }
            }

            Context.SaveChanges();
        }
    }
}
