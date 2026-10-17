using Domain.Entities;
using Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests
{
    // The EF in-memory provider does not support ExecuteDelete, so the tests run on an in-memory SQLite database.
    public class SqliteTestDatabase : IDisposable
    {
        private readonly SqliteConnection connection;

        public SqliteTestDatabase()
        {
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection).Options;
            Context = new ApplicationDbContext(options);
            Context.Database.EnsureCreated();
            SeedData();
        }

        public ApplicationDbContext Context { get; }

        public void Dispose()
        {
            Context.Dispose();
            connection.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Recreate()
        {
            Context.ChangeTracker.Clear();
            Context.Contacts.ExecuteDelete();
            Context.Users.ExecuteDelete();
            SeedData();
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
