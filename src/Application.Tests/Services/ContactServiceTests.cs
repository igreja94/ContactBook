using Application.DTO;
using Application.Services;
using Domain.Entities;
using Application.Repositories;
using Moq;

namespace Application.Tests.Services
{
	public class ContactServiceTests : IAsyncLifetime
	{
		private readonly Mock<IContactRepository> MockContactRepo = new();
		private readonly ContactService _service;
		private Guid UserId1 = Guid.NewGuid();
		private Guid UserId2 = Guid.NewGuid();

		// Sample data
		private List<Contact> Contacts = [];

		public async Task InitializeAsync()
		{
			Contacts =
			[
				new() { Id = Guid.NewGuid(), FirstName = "Test", LastName = "Contact1", DateOfBirth = DateTime.MinValue, Email = "contact1@test.com", PhoneNumber = "123456", UserId = UserId1 },
				new() { Id = Guid.NewGuid(), FirstName = "Test", LastName = "Contact2", DateOfBirth = DateTime.MinValue, Email = "contact2@test.com", PhoneNumber = "654321", UserId = UserId2 }
			];

			MockContactRepo.Setup(r => r.GetAllAsync())
				.ReturnsAsync(Contacts);

			MockContactRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
				.ReturnsAsync((Guid id) => Contacts.FirstOrDefault(c => c.Id == id)!);

			MockContactRepo.Setup(r => r.AddAsync(It.IsAny<Contact>()))
				.Returns(Task.CompletedTask)
				.Callback((Contact contact) => Contacts.Add(contact));

			MockContactRepo.Setup(r => r.GetContactsByUserAsync(It.IsAny<Guid>()))
				.ReturnsAsync((Guid userId) => Contacts.Where(c => c.UserId == userId));
		}

		public Task DisposeAsync()
		{
			return Task.CompletedTask;
		}

		public ContactServiceTests()
		{
			_service = new ContactService(MockContactRepo.Object);
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturnAllContacts()
		{
			// Act
			var result = await _service.GetAllAsync();
			// Assert
			Assert.Equal(Contacts.Count, result.Count());
			foreach (var contact in Contacts)
			{
				Assert.Contains(result, r => r.Id == contact.Id.ToString() && r.FirstName == contact.FirstName && r.LastName == contact.LastName);
			}
		}

		[Fact]
		public async Task GetByIdAsync_ShouldReturnCorrectContact()
		{
			// Arrange
			var targetContact = Contacts.First();
			// Act
			var result = await _service.GetByIdAsync(targetContact.Id.ToString());
			// Assert
			Assert.NotNull(result);
			Assert.Equal(targetContact.Id.ToString(), result.Id);
			Assert.Equal(targetContact.FirstName, result.FirstName);
			Assert.Equal(targetContact.LastName, result.LastName);
		}

		[Fact]
		public async Task GetByIdAsync_ShouldThrow_WhenContactNotFound()
		{
			// Arrange
			var nonExistentId = Guid.NewGuid().ToString();
			MockContactRepo.Setup(r => r.GetByIdAsync(Guid.Parse(nonExistentId)))
				.ThrowsAsync(new InvalidOperationException("Sequence contains no matching element"));

			// Act & Assert
			await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetByIdAsync(nonExistentId));
		}

		[Fact]
		public async Task AddAsync_ShouldAddContact()
		{
			// Arrange
			var newContactDto = new ContactDto
			{
				Id = Guid.NewGuid().ToString(),
				FirstName = "New",
				LastName = "Contact",
				DateOfBirth = DateTime.MinValue,
				Email = "new@test.com",
				PhoneNumber = "999999"
			};
			// Act
			await _service.AddAsync(newContactDto);
			// Assert
			MockContactRepo.Verify(r => r.AddAsync(It.IsAny<Contact>()), Times.Once);
			MockContactRepo.Verify(r => r.AddAsync(It.Is<Contact>(c => c.Id == Guid.Parse(newContactDto.Id))), Times.Once);
			MockContactRepo.Verify(r => r.AddAsync(It.Is<Contact>(c => c.FirstName == newContactDto.FirstName)), Times.Once);
		}

		[Fact]
		public async Task UpdateAsync_ShouldUpdateContact()
		{
			// Arrange
			var existingContact = Contacts.First();
			var updatedContactDto = new ContactDto
			{
				Id = existingContact.Id.ToString(),
				FirstName = "Updated",
				LastName = existingContact.LastName,
				DateOfBirth = existingContact.DateOfBirth,
				Email = existingContact.Email,
				PhoneNumber = existingContact.PhoneNumber
			};
			// Act
			await _service.UpdateAsync(updatedContactDto);
			// Assert
			MockContactRepo.Verify(r => r.UpdateAsync(It.IsAny<Contact>()), Times.Once);
			MockContactRepo.Verify(r => r.UpdateAsync(It.Is<Contact>(c => c.Id == Guid.Parse(updatedContactDto.Id) && c.FirstName == updatedContactDto.FirstName)), Times.Once);

		}

		[Fact]
		public async Task DeleteAsync_ShouldDeleteContact()
		{
			// Arrange
			var existingContact = Contacts.First();
			// Act
			await _service.DeleteAsync(existingContact.Id.ToString());
			// Assert
			MockContactRepo.Verify(r => r.DeleteAsync(It.IsAny<Contact>()), Times.Once);
			MockContactRepo.Verify(r => r.DeleteAsync(It.Is<Contact>(c => c.Id == existingContact.Id)), Times.Once);

		}

		[Fact]
		public async Task GetContactsByUserId_ShouldReturnContactsForUser()
		{
			// Arrange
			var userId = UserId1.ToString();
			var userGuid = UserId1;
			var userContacts = new List<Contact>
			{
				new() { Id = Guid.NewGuid(), FirstName = "Contact", LastName = "A", DateOfBirth = DateTime.MinValue, Email = "a@test.com", PhoneNumber = "111111", UserId = userGuid },
				new() { Id = Guid.NewGuid(), FirstName = "Contact", LastName = "B", DateOfBirth = DateTime.MinValue, Email = "b@test.com", PhoneNumber = "222222", UserId = userGuid }
			};

			MockContactRepo.Setup(r => r.GetContactsByUserAsync(userGuid))
				.ReturnsAsync(userContacts);

			// Act
			var result = await _service.GetContactsByUserId(userId);

			// Assert
			Assert.Equal(userContacts.Count, result.Count());
			foreach (var contact in userContacts)
			{
				Assert.Contains(result, r => r.Id == contact.Id.ToString() && r.FirstName == contact.FirstName && r.LastName == contact.LastName);
			}
		}

	}
}
