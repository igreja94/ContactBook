using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests.Repositories
{
    public class BaseRepositoryTests(InMemoryDbContext context) : IClassFixture<InMemoryDbContext>, IAsyncLifetime
    {
        // Despite BaseRepository being abstract, we implement a simple concrete class for testing working with User entities.
        private class TestableBaseRepository(ApplicationDbContext context) : BaseRepository<User>(context);

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
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            //Arrange
            var repo = new TestableBaseRepository(Context.Context);

            //Act
            var entities = await repo.GetAllAsync();

            //Assert
            Assert.Equal(5, entities.Count());
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyCollection_WhenNoEntities()
        {
            //Arrange
            var repo = new TestableBaseRepository(Context.Context);
            Context.Context.Users.RemoveRange(Context.Context.Users);
            Context.Context.SaveChanges();

            //Act
            var entities = await repo.GetAllAsync();
            //Assert
            Assert.Empty(entities);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectEntity()
        {
            //Arrange
            var repo = new TestableBaseRepository(Context.Context);
            var existingUser = Context.Context.Users.First();

            //Act
            var entity = await repo.GetByIdAsync(existingUser.Id);

            //Assert
            Assert.NotNull(entity);
            Assert.Equal(existingUser.Id, entity.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowException_WhenEntityNotFound()
        {
            //Arrange
            var repo = new TestableBaseRepository(Context.Context);
            var nonExistentId = Guid.NewGuid();

            //Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await repo.GetByIdAsync(nonExistentId));
        }

        [Fact]
        public async Task AddAsync_ShouldAddEntity()
        {
            //Arrange
            var repo = new TestableBaseRepository(Context.Context);
            var newUser = new User();
            Guid newUserId = newUser.Id;

            //Act
            await repo.AddAsync(newUser);
            var addedUser = await repo.GetByIdAsync(newUserId);

            //Assert
            Assert.NotNull(addedUser);
            Assert.Equal(newUserId, addedUser.Id);
        }

        [Fact]
        public async Task AddAsync_ShouldThrowException_WhenModelIsNull()
        {
            //Arrange
            var repo = new TestableBaseRepository(Context.Context);

            //Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await repo.AddAsync(null!));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEntity()
        {
            //Arrange
            var repo = new TestableBaseRepository(Context.Context);
            var existingUser = Context.Context.Users.First();
            existingUser.FirstName = "UpdatedName";

            //Act
            await repo.UpdateAsync(existingUser);
            var updatedUser = await repo.GetByIdAsync(existingUser.Id);

            //Assert
            Assert.NotNull(updatedUser);
            Assert.Equal("UpdatedName", updatedUser.FirstName);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenModelIsNull()
        {
            //Arrange
            var repo = new TestableBaseRepository(Context.Context);

            //Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await repo.UpdateAsync(null!));
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveEntity()
        {
            //Arrange
            var repo = new TestableBaseRepository(Context.Context);
            var existingUser = Context.Context.Users.First();

            //Act
            await repo.DeleteAsync(existingUser);

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await repo.GetByIdAsync(existingUser.Id));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenModelIsNull()
        {
            //Arrange
            var repo = new TestableBaseRepository(Context.Context);

            //Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await repo.DeleteAsync(null!));
        }
    }
}
