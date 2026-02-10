using Domain.Entities;
using Infrastructure.Repositories;

namespace Infrastructure.Tests.Repositories
{
    public class ContactRepositoryTests(InMemoryDbContext context) : IClassFixture<InMemoryDbContext>, IAsyncLifetime
    {
        private readonly InMemoryDbContext Context = context;

        public Task InitializeAsync()
        {
            Context.Recreate();
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task GetContactsByUser_ShouldReturnContacts_ForExistingUserWithContacts()
        {
            //Arrange
            var repo = new ContactRepository(Context.Context);
            var existingUser = Context.Context.Users.First();

            //Act
            var contacts = await repo.GetContactsByUserAsync(existingUser.Id);

            //Assert
            Assert.Equal(5, contacts.Count());
        }

        [Fact]
        public async Task GetContactsByUser_ShouldReturEmpty_ForExistingUserWithNoContacts()
        {
            //Arrange
            var userRepo = new UserRepository(Context.Context);
            var contactRepo = new ContactRepository(Context.Context);

            User user = new()
            {
                FirstName = "New",
                LastName = "User",
            };

            await userRepo.AddAsync(user);

            //Act
            var contacts = await contactRepo.GetContactsByUserAsync(user.Id);

            //Assert
            Assert.Empty(contacts);
        }

        [Fact]
        public async Task GetContactsByUser_ShouldNotReturnContacts_ForNonExistingUser()
        {
            //Arrange
            var repo = new ContactRepository(Context.Context);
            var existingUser = Context.Context.Users.First();
            Guid guid = Guid.NewGuid();

            //Act
            var contacts = await repo.GetContactsByUserAsync(guid);

            //Assert
            Assert.Empty(contacts);
        }

        [Fact]
        public async Task DeleteContactsByUser_ShouldDeleteContacts_ForExistingUserWithContacts()
        {
            //Arrange
            var repo = new ContactRepository(Context.Context);
            var existingUser = Context.Context.Users.First();
            var contactIdsToDelete = Context.Context.Contacts
                .Where(c => c.UserId == existingUser.Id).Take(2).Select(c => c.Id).ToList();

            //Act
            await repo.DeleteContactsByUserAsync(existingUser.Id, contactIdsToDelete);

            //Assert
            var remainingContacts = await repo.GetContactsByUserAsync(existingUser.Id);
            Assert.Equal(3, remainingContacts.Count());
        }

        [Fact]
        public async Task DeleteContactsByUser_ShouldNotDeleteContacts_ForExistingUserWithContacts_WhenContactIdsDoNotMatch()
        {
            //Arrange
            var repo = new ContactRepository(Context.Context);
            var existingUser = Context.Context.Users.First();
            var nonExistingContactIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            //Act
            await repo.DeleteContactsByUserAsync(existingUser.Id, nonExistingContactIds);
            //Assert
            var remainingContacts = await repo.GetContactsByUserAsync(existingUser.Id);
            Assert.Equal(5, remainingContacts.Count());
        }

        [Fact]
        public async Task DeleteContactsByUser_ShouldNotDeleteContacts_ForNonExistingUser()
        {
            //Arrange
            var repo = new ContactRepository(Context.Context);
            Guid nonExistingUserId = Guid.NewGuid();
            var contactIdsToDelete = Context.Context.Contacts.Select(c => c.Id).Take(2).ToList();
            //Act

            await repo.DeleteContactsByUserAsync(nonExistingUserId, contactIdsToDelete);
            //Assert

            var remainingContacts = await repo.GetContactsByUserAsync(nonExistingUserId);
            Assert.Empty(remainingContacts);
        }
    }
}
