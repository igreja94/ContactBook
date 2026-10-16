using Application.DTO;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Moq;

namespace Application.Tests.Services
{
    public class UserServiceTests : IAsyncLifetime
    {
        private readonly Mock<IUserRepository> mockUserRepo = new();
        private readonly UserService service;

        // Sample data
        private readonly List<User> users =
        [
            new() { Id = Guid.NewGuid(), FirstName = "Test", LastName = "User1" },
            new() { Id = Guid.NewGuid(), FirstName = "Test", LastName = "User2" }
        ];

        public UserServiceTests()
        {
            service = new UserService(mockUserRepo.Object);
        }

        public async Task InitializeAsync()
        {
            mockUserRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(users);

            mockUserRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => users.First(u => u.Id == id));

            mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask)
                .Callback((User user) => users.Add(user));
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Equal(users.Count, result.Count());
            foreach (var user in users)
            {
                Assert.Contains(result, r => r.Id == user.Id && r.FirstName == user.FirstName && r.LastName == user.LastName);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectUser()
        {
            // Arrange
            var targetUser = users.First();

            // Act
            var result = await service.GetByIdAsync(targetUser.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(targetUser.Id, result.Id);
            Assert.Equal(targetUser.FirstName, result.FirstName);
            Assert.Equal(targetUser.LastName, result.LastName);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrow_WhenUserNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetByIdAsync(nonExistentId));
        }

        [Fact]
        public async Task AddAsync_ShouldAddUser()
        {
            // Arrange
            var newUserDto = new UserDto { Id = Guid.NewGuid(), FirstName = "New", LastName = "User" };

            // Act
            var result = await service.AddAsync(newUserDto);

            // Assert
            Assert.NotEqual(newUserDto.Id, result.Id);
            Assert.NotEqual(Guid.Empty, result.Id!.Value);
            Assert.Equal(newUserDto.FirstName, result.FirstName);
            mockUserRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
            mockUserRepo.Verify(r => r.AddAsync(It.Is<User>(u => u.Id == result.Id!.Value)), Times.Once);
            mockUserRepo.Verify(r => r.AddAsync(It.Is<User>(u => u.FirstName == newUserDto.FirstName)), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUser()
        {
            // Arrange
            var existingUser = users.First();
            var updatedUserDto = new UserDto
            {
                Id = existingUser.Id,
                FirstName = "Updated",
                LastName = existingUser.LastName,
            };

            // Act
            await service.UpdateAsync(updatedUserDto);

            // Assert
            mockUserRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
            mockUserRepo.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Id == updatedUserDto.Id && u.FirstName == updatedUserDto.FirstName)), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteUser()
        {
            // Arrange
            var existingUser = users.First();

            // Act
            await service.DeleteAsync(existingUser.Id);

            // Assert
            mockUserRepo.Verify(r => r.DeleteAsync(It.IsAny<User>()), Times.Once);
            mockUserRepo.Verify(r => r.DeleteAsync(It.Is<User>(u => u.Id == existingUser.Id)), Times.Once);
        }
    }
}
