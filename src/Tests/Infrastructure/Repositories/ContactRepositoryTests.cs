using Domain.Entities;
using Infrastructure.Repositories;

namespace Infrastructure.Tests.Repositories
{
    public class ContactRepositoryTests : IClassFixture<SqliteTestDatabase>, IAsyncLifetime
    {
        private readonly SqliteTestDatabase context;
        private readonly ContactRepository repo;

        public ContactRepositoryTests(SqliteTestDatabase context)
        {
            this.context = context;
            repo = new ContactRepository(context.Context);
        }

        async Task IAsyncLifetime.InitializeAsync()
        {
            context.Recreate();
            await Task.CompletedTask;
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await Task.CompletedTask;
        }

        [Fact]
        public async Task GetContactsByUser_ShouldReturnContacts_ForExistingUserWithContacts()
        {
            // Arrange
            var existingUser = context.Context.Users.First();

            // Act
            var contacts = await repo.GetContactsByUserAsync(existingUser.Id);

            // Assert
            Assert.Equal(5, contacts.Count());
        }

        [Fact]
        public async Task GetContactsByUser_ShouldReturEmpty_ForExistingUserWithNoContacts()
        {
            // Arrange
            var userRepo = new UserRepository(context.Context);

            User user = new()
            {
                FirstName = "New",
                LastName = "User",
            };

            await userRepo.AddAsync(user);

            // Act
            var contacts = await repo.GetContactsByUserAsync(user.Id);

            // Assert
            Assert.Empty(contacts);
        }

        [Fact]
        public async Task GetContactsByUser_ShouldNotReturnContacts_ForNonExistingUser()
        {
            // Arrange
            var nonExistingUserId = Guid.NewGuid();

            // Act
            var contacts = await repo.GetContactsByUserAsync(nonExistingUserId);

            // Assert
            Assert.Empty(contacts);
        }
    }
}
