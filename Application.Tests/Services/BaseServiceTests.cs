using Application.DTO;
using Application.Services;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Moq;

namespace Application.Tests.Services
{
	public class BaseServiceTests : IAsyncLifetime
	{
		private readonly Mock<IRepository<User>> MockUserRepo = new();
		private readonly UserService _service;

		// Sample data
		private readonly List<User> Users =
		[
			new() { Id = Guid.NewGuid(), FirstName = "Test", LastName = "User1" },
			new() { Id = Guid.NewGuid(), FirstName = "Test", LastName = "User2" }
		];

		public async Task InitializeAsync()
		{

			MockUserRepo.Setup(r => r.GetAllAsync())
				.ReturnsAsync(Users);

			MockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
				.ReturnsAsync((Guid id) => Users.First(u => u.Id == id));

			MockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>()))
				.Returns(Task.CompletedTask)
				.Callback((User user) => Users.Add(user));
		}

		public Task DisposeAsync()
		{
			return Task.CompletedTask;
		}

		public BaseServiceTests()
		{
			_service = new UserService(MockUserRepo.Object);
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturnAllUsers()
		{
			// Act
			var result = await _service.GetAllAsync();
			// Assert
			Assert.Equal(Users.Count, result.Count());
			foreach (var user in Users)
			{
				Assert.Contains(result, r => r.Id == user.Id.ToString() && r.FirstName == user.FirstName && r.LastName == user.LastName);
			}
		}

		[Fact]
		public async Task GetByIdAsync_ShouldReturnCorrectUser()
		{
			// Arrange
			var targetUser = Users.First();
			// Act
			var result = await _service.GetByIdAsync(targetUser.Id.ToString());
			// Assert
			Assert.NotNull(result);
			Assert.Equal(targetUser.Id.ToString(), result.Id);
			Assert.Equal(targetUser.FirstName, result.FirstName);
			Assert.Equal(targetUser.LastName, result.LastName);
		}

		[Fact]
		public async Task GetByIdAsync_ShouldThrow_WhenUserNotFound()
		{
			// Arrange
			var nonExistentId = Guid.NewGuid().ToString();

			// Act & Assert
			await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetByIdAsync(nonExistentId));
		}

		[Fact]
		public async Task AddAsync_ShouldAddUser()
		{
			// Arrange
			var newUserDto = new UserDto { Id = Guid.NewGuid().ToString(), FirstName = "New", LastName = "User" };
			// Act
			await _service.AddAsync(newUserDto);
			// Assert
			MockUserRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
			MockUserRepo.Verify(r => r.AddAsync(It.Is<User>(u => u.Id == Guid.Parse(newUserDto.Id))), Times.Once);
			MockUserRepo.Verify(r => r.AddAsync(It.Is<User>(u => u.FirstName == newUserDto.FirstName)), Times.Once);
		}

		[Fact]
		public async Task UpdateAsync_ShouldUpdateUser()
		{
			// Arrange
			var existingUser = Users.First();
			var updatedUserDto = new UserDto
			{
				Id = existingUser.Id.ToString(),
				FirstName = "Updated",
				LastName = existingUser.LastName
			};
			// Act
			await _service.UpdateAsync(updatedUserDto);
			// Assert
			MockUserRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
			MockUserRepo.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Id == Guid.Parse(updatedUserDto.Id) && u.FirstName == updatedUserDto.FirstName)), Times.Once);

		}

		[Fact]
		public async Task DeleteAsync_ShouldDeleteUser()
		{
			// Arrange
			var existingUser = Users.First();
			// Act
			await _service.DeleteAsync(existingUser.Id.ToString());
			// Assert
			MockUserRepo.Verify(r => r.DeleteAsync(It.IsAny<User>()), Times.Once);
			MockUserRepo.Verify(r => r.DeleteAsync(It.Is<User>(u => u.Id == existingUser.Id)), Times.Once);

		}

	}
}