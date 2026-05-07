using Domain.Entities;
using Infrastructure.Repositories;

namespace Infrastructure.Tests.Repositories
{
	public class UserRepositoryTests : IClassFixture<InMemoryDbContext>, IAsyncLifetime
	{
		private readonly InMemoryDbContext _context;
		private readonly UserRepository _repo;

		public UserRepositoryTests(InMemoryDbContext context)
		{
			_context = context;
			_repo = new UserRepository(context.Context);
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
		public async Task GetAllAsync_ShouldReturnAllEntities()
		{
			//Act
			var entities = await _repo.GetAllAsync();

			//Assert
			Assert.Equal(5, entities.Count());
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturnEmptyCollection_WhenNoEntities()
		{
			//Arrange
			_context.Context.Users.RemoveRange(_context.Context.Users);
			_context.Context.SaveChanges();

			//Act
			var entities = await _repo.GetAllAsync();
			//Assert
			Assert.Empty(entities);
		}

		[Fact]
		public async Task GetByIdAsync_ShouldReturnCorrectEntity()
		{
			//Arrange
			var existingUser = _context.Context.Users.First();

			//Act
			var entity = await _repo.GetByIdAsync(existingUser.Id);

			//Assert
			Assert.NotNull(entity);
			Assert.Equal(existingUser.Id, entity.Id);
		}

		[Fact]
		public async Task GetByIdAsync_ShouldThrowException_WhenEntityNotFound()
		{
			//Arrange
			var nonExistentId = Guid.NewGuid();

			//Act & Assert
			await Assert.ThrowsAsync<InvalidOperationException>(async () => await _repo.GetByIdAsync(nonExistentId));
		}

		[Fact]
		public async Task AddAsync_ShouldAddEntity()
		{
			//Arrange
			var newUser = new User();
			Guid newUserId = newUser.Id;

			//Act
			await _repo.AddAsync(newUser);
			var addedUser = await _repo.GetByIdAsync(newUserId);

			//Assert
			Assert.NotNull(addedUser);
			Assert.Equal(newUserId, addedUser.Id);
		}

		[Fact]
		public async Task AddAsync_ShouldThrowException_WhenModelIsNull()
		{
			//Act & Assert
			await Assert.ThrowsAsync<ArgumentNullException>(async () => await _repo.AddAsync(null!));
		}

		[Fact]
		public async Task UpdateAsync_ShouldUpdateEntity()
		{
			//Arrange
			var existingUser = _context.Context.Users.First();
			existingUser.FirstName = "UpdatedName";

			//Act
			await _repo.UpdateAsync(existingUser);
			var updatedUser = await _repo.GetByIdAsync(existingUser.Id);

			//Assert
			Assert.NotNull(updatedUser);
			Assert.Equal("UpdatedName", updatedUser.FirstName);
		}

		[Fact]
		public async Task UpdateAsync_ShouldThrowException_WhenModelIsNull()
		{
			//Act & Assert
			await Assert.ThrowsAsync<ArgumentNullException>(async () => await _repo.UpdateAsync(null!));
		}

		[Fact]
		public async Task DeleteAsync_ShouldRemoveEntity()
		{
			//Arrange
			var existingUser = _context.Context.Users.First();

			//Act
			await _repo.DeleteAsync(existingUser);

			//Assert
			await Assert.ThrowsAsync<InvalidOperationException>(async () => await _repo.GetByIdAsync(existingUser.Id));
		}

		[Fact]
		public async Task DeleteAsync_ShouldThrowException_WhenModelIsNull()
		{
			//Act & Assert
			await Assert.ThrowsAsync<ArgumentNullException>(async () => await _repo.DeleteAsync(null!));
		}
	}
}
