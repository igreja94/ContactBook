using Application.DTO;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Moq;

namespace Application.Tests.Services
{
    public class ContactServiceTests : IAsyncLifetime
    {
        private readonly Mock<IContactRepository> mockContactRepo = new();
        private readonly ContactService service;
        private readonly Guid userId1 = Guid.NewGuid();
        private readonly Guid userId2 = Guid.NewGuid();

        // Sample data
        private List<Contact> contacts = [];

        public ContactServiceTests()
        {
            service = new ContactService(mockContactRepo.Object);
        }

        public async Task InitializeAsync()
        {
            contacts =
            [
                new() { Id = Guid.NewGuid(), FirstName = "Test", LastName = "Contact1", DateOfBirth = DateTime.MinValue, Email = "contact1@test.com", PhoneNumber = "123456", UserId = userId1 },
                new() { Id = Guid.NewGuid(), FirstName = "Test", LastName = "Contact2", DateOfBirth = DateTime.MinValue, Email = "contact2@test.com", PhoneNumber = "654321", UserId = userId2 }
            ];

            mockContactRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(contacts);

            mockContactRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => contacts.FirstOrDefault(c => c.Id == id)!);

            mockContactRepo.Setup(r => r.AddAsync(It.IsAny<Contact>()))
                .Returns(Task.CompletedTask)
                .Callback((Contact contact) => contacts.Add(contact));

            mockContactRepo.Setup(r => r.GetContactsByUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid userId) => contacts.Where(c => c.UserId == userId));
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllContacts()
        {
            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Equal(contacts.Count, result.Count());
            foreach (var contact in contacts)
            {
                Assert.Contains(result, r => r.Id == contact.Id && r.FirstName == contact.FirstName && r.LastName == contact.LastName);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectContact()
        {
            // Arrange
            var targetContact = contacts.First();

            // Act
            var result = await service.GetByIdAsync(targetContact.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(targetContact.Id, result.Id);
            Assert.Equal(targetContact.FirstName, result.FirstName);
            Assert.Equal(targetContact.LastName, result.LastName);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrow_WhenContactNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            mockContactRepo.Setup(r => r.GetByIdAsync(nonExistentId))
                .ThrowsAsync(new InvalidOperationException("Sequence contains no matching element"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetByIdAsync(nonExistentId));
        }

        [Fact]
        public async Task AddAsync_ShouldAddContact()
        {
            // Arrange
            var newContactDto = new ContactDto
            {
                Id = Guid.NewGuid(),
                FirstName = "New",
                LastName = "Contact",
                DateOfBirth = DateTime.MinValue,
                Email = "new@test.com",
                PhoneNumber = "999999",
            };

            // Act
            var result = await service.AddAsync(newContactDto);

            // Assert
            Assert.NotEqual(newContactDto.Id, result.Id);
            Assert.NotEqual(Guid.Empty, result.Id!.Value);
            Assert.Equal(newContactDto.FirstName, result.FirstName);
            mockContactRepo.Verify(r => r.AddAsync(It.IsAny<Contact>()), Times.Once);
            mockContactRepo.Verify(r => r.AddAsync(It.Is<Contact>(c => c.Id == result.Id!.Value)), Times.Once);
            mockContactRepo.Verify(r => r.AddAsync(It.Is<Contact>(c => c.FirstName == newContactDto.FirstName)), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateContact()
        {
            // Arrange
            var existingContact = contacts.First();
            var updatedContactDto = new ContactDto
            {
                Id = existingContact.Id,
                FirstName = "Updated",
                LastName = existingContact.LastName,
                DateOfBirth = existingContact.DateOfBirth,
                Email = existingContact.Email,
                PhoneNumber = existingContact.PhoneNumber,
            };

            // Act
            await service.UpdateAsync(updatedContactDto);

            // Assert
            mockContactRepo.Verify(r => r.UpdateAsync(It.IsAny<Contact>()), Times.Once);
            mockContactRepo.Verify(r => r.UpdateAsync(It.Is<Contact>(c => c.Id == updatedContactDto.Id && c.FirstName == updatedContactDto.FirstName)), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteContact()
        {
            // Arrange
            var existingContact = contacts.First();

            // Act
            await service.DeleteAsync(existingContact.Id);

            // Assert
            mockContactRepo.Verify(r => r.DeleteAsync(It.IsAny<Contact>()), Times.Once);
            mockContactRepo.Verify(r => r.DeleteAsync(It.Is<Contact>(c => c.Id == existingContact.Id)), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldPreserveOwner()
        {
            // Arrange
            var existingContact = contacts.First();
            var ownerId = existingContact.UserId;
            var dto = new ContactDto
            {
                Id = existingContact.Id,
                FirstName = "Updated",
                LastName = existingContact.LastName,
                DateOfBirth = existingContact.DateOfBirth,
                UserId = Guid.NewGuid(),
            };

            // Act
            await service.UpdateAsync(dto);

            // Assert
            mockContactRepo.Verify(r => r.UpdateAsync(It.Is<Contact>(c => c.UserId == ownerId)), Times.Once);
        }

        [Fact]
        public async Task DeleteContactsByUserAsync_ShouldDelegateToRepository()
        {
            // Arrange
            var contactIds = contacts.Select(c => c.Id).ToList();

            // Act
            await service.DeleteContactsByUserAsync(userId1, contactIds);

            // Assert
            mockContactRepo.Verify(r => r.DeleteContactsByUserAsync(userId1, It.Is<ICollection<Guid>>(ids => ids.SequenceEqual(contactIds))), Times.Once);
        }

        [Fact]
        public async Task GetContactsByUserId_ShouldReturnContactsForUser()
        {
            // Arrange
            var userGuid = userId1;
            var userContacts = new List<Contact>
            {
                new() { Id = Guid.NewGuid(), FirstName = "Contact", LastName = "A", DateOfBirth = DateTime.MinValue, Email = "a@test.com", PhoneNumber = "111111", UserId = userGuid },
                new() { Id = Guid.NewGuid(), FirstName = "Contact", LastName = "B", DateOfBirth = DateTime.MinValue, Email = "b@test.com", PhoneNumber = "222222", UserId = userGuid },
            };

            mockContactRepo.Setup(r => r.GetContactsByUserAsync(userGuid))
                .ReturnsAsync(userContacts);

            // Act
            var result = await service.GetContactsByUserId(userGuid);

            // Assert
            Assert.Equal(userContacts.Count, result.Count());
            foreach (var contact in userContacts)
            {
                Assert.Contains(result, r => r.Id == contact.Id && r.FirstName == contact.FirstName && r.LastName == contact.LastName);
            }
        }
    }
}
