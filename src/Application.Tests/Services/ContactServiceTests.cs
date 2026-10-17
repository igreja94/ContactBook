using Application.DTO;
using Application.Exceptions;
using Application.Mappers;
using Application.Repositories.Interfaces;
using Application.Services;
using Domain.Entities;
using Moq;

namespace Application.Tests.Services
{
    public class ContactServiceTests : IAsyncLifetime
    {
        private readonly Mock<IContactRepository> mockContactRepo = new();
        private readonly Mock<IUserRepository> mockUserRepo = new();
        private readonly ContactService service;
        private readonly Guid userId1 = Guid.NewGuid();
        private readonly Guid userId2 = Guid.NewGuid();

        // Sample data
        private List<Contact> contacts = [];

        public ContactServiceTests()
        {
            service = new ContactService(mockContactRepo.Object, mockUserRepo.Object, new ContactMapper());
        }

        public async Task InitializeAsync()
        {
            contacts =
            [
                new() { Id = Guid.NewGuid(), FirstName = "Test", LastName = "Contact1", DateOfBirth = DateTime.MinValue, Email = "contact1@test.com", PhoneNumber = "123456", UserId = userId1 },
                new() { Id = Guid.NewGuid(), FirstName = "Test", LastName = "Contact2", DateOfBirth = DateTime.MinValue, Email = "contact2@test.com", PhoneNumber = "654321", UserId = userId2 }
            ];

            mockUserRepo.Setup(r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken _) => id == userId1 || id == userId2);

            mockContactRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(contacts);

            mockContactRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken _) => contacts.FirstOrDefault(c => c.Id == id)!);

            mockContactRepo.Setup(r => r.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback((Contact contact, CancellationToken _) => contacts.Add(contact));

            mockContactRepo.Setup(r => r.GetContactsByUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid userId, CancellationToken _) => contacts.Where(c => c.UserId == userId));
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
            mockContactRepo.Setup(r => r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Contact not found."));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(nonExistentId));
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
                UserId = userId1,
            };

            // Act
            var result = await service.AddAsync(newContactDto);

            // Assert
            Assert.NotEqual(newContactDto.Id, result.Id);
            Assert.NotEqual(Guid.Empty, result.Id!.Value);
            Assert.Equal(newContactDto.FirstName, result.FirstName);
            mockContactRepo.Verify(r => r.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()), Times.Once);
            mockContactRepo.Verify(r => r.AddAsync(It.Is<Contact>(c => c.Id == result.Id!.Value), It.IsAny<CancellationToken>()), Times.Once);
            mockContactRepo.Verify(r => r.AddAsync(It.Is<Contact>(c => c.FirstName == newContactDto.FirstName), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldThrowValidation_WhenUserDoesNotExist()
        {
            // Arrange
            var dto = new ContactDto { FirstName = "New", LastName = "Contact", UserId = Guid.NewGuid() };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationFailedException>(() => service.AddAsync(dto));
            Assert.Equal(nameof(ContactDto.UserId), exception.PropertyName);
            mockContactRepo.Verify(r => r.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddAsync_ShouldThrowValidation_WhenUserIdIsMissing()
        {
            // Arrange
            var dto = new ContactDto { FirstName = "New", LastName = "Contact" };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationFailedException>(() => service.AddAsync(dto));
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
            mockContactRepo.Verify(r => r.UpdateAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()), Times.Once);
            mockContactRepo.Verify(r => r.UpdateAsync(It.Is<Contact>(c => c.Id == updatedContactDto.Id && c.FirstName == updatedContactDto.FirstName), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteContact()
        {
            // Arrange
            var existingContact = contacts.First();

            // Act
            await service.DeleteAsync(existingContact.Id);

            // Assert
            mockContactRepo.Verify(r => r.DeleteAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()), Times.Once);
            mockContactRepo.Verify(r => r.DeleteAsync(It.Is<Contact>(c => c.Id == existingContact.Id), It.IsAny<CancellationToken>()), Times.Once);
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
            mockContactRepo.Verify(r => r.UpdateAsync(It.Is<Contact>(c => c.UserId == ownerId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetContactsByUserId_ShouldThrowNotFound_WhenUserDoesNotExist()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.GetContactsByUserId(Guid.NewGuid()));
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

            mockContactRepo.Setup(r => r.GetContactsByUserAsync(userGuid, It.IsAny<CancellationToken>()))
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
