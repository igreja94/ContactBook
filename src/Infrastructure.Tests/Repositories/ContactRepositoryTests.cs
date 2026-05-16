using Domain.Entities;
using Infrastructure.Repositories;

namespace Infrastructure.Tests.Repositories
{
    public class ContactRepositoryTests : IClassFixture<InMemoryDbContext>, IAsyncLifetime
    {
        private readonly InMemoryDbContext _context;
        private readonly ContactRepository _repo;

        public ContactRepositoryTests(InMemoryDbContext context)
        {
            _context = context;
            _repo = new ContactRepository(context.Context);
        }

        async Task IAsyncLifetime.InitializeAsync()
        {
            _context.Recreate();
            await Task.CompletedTask;
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await Task.CompletedTask;
        }

        [Fact]
        public async Task GetContactsByUser_ShouldReturnContacts_ForExistingUserWithContacts()
        {
            //Arrange
            var existingUser = _context.Context.Users.First();

            //Act
            var contacts = await _repo.GetContactsByUserAsync(existingUser.Id);

            //Assert
            Assert.Equal(5, contacts.Count());
        }

        [Fact]
        public async Task GetContactsByUser_ShouldReturEmpty_ForExistingUserWithNoContacts()
        {
            //Arrange
            var userRepo = new UserRepository(_context.Context);

            User user = new()
            {
                FirstName = "New",
                LastName = "User",
            };

            await userRepo.AddAsync(user);

            //Act
            var contacts = await _repo.GetContactsByUserAsync(user.Id);

            //Assert
            Assert.Empty(contacts);
        }

        [Fact]
        public async Task GetContactsByUser_ShouldNotReturnContacts_ForNonExistingUser()
        {
            //Arrange
            var nonExistingUserId = Guid.NewGuid();

            //Act
            var contacts = await _repo.GetContactsByUserAsync(nonExistingUserId);

            //Assert
            Assert.Empty(contacts);
        }

        [Fact]
        public async Task DeleteContactsByUser_ShouldDeleteContacts_ForExistingUserWithContacts()
        {
            //Arrange
            var existingUser = _context.Context.Users.First();
            var contactIdsToDelete = _context.Context.Contacts
                .Where(c => c.UserId == existingUser.Id).Take(2).Select(c => c.Id).ToList();

            //Act
            await _repo.DeleteContactsByUserAsync(existingUser.Id, contactIdsToDelete);

            //Assert
            var remainingContacts = await _repo.GetContactsByUserAsync(existingUser.Id);
            Assert.Equal(3, remainingContacts.Count());
        }

        [Fact]
        public async Task DeleteContactsByUser_ShouldNotDeleteContacts_ForExistingUserWithContacts_WhenContactIdsDoNotMatch()
        {
            //Arrange
            var existingUser = _context.Context.Users.First();
            var nonExistingContactIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            //Act
            await _repo.DeleteContactsByUserAsync(existingUser.Id, nonExistingContactIds);

            //Assert
            var remainingContacts = await _repo.GetContactsByUserAsync(existingUser.Id);
            Assert.Equal(5, remainingContacts.Count());
        }

        [Fact]
        public async Task DeleteContactsByUser_ShouldNotDeleteContacts_ForNonExistingUser()
        {
            //Arrange
            Guid nonExistingUserId = Guid.NewGuid();
            var contactIdsToDelete = _context.Context.Contacts.Select(c => c.Id).Take(2).ToList();

            //Act
            await _repo.DeleteContactsByUserAsync(nonExistingUserId, contactIdsToDelete);

            //Assert
            var remainingContacts = await _repo.GetContactsByUserAsync(nonExistingUserId);
            Assert.Empty(remainingContacts);
        }
    }
}
