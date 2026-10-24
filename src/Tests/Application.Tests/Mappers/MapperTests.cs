using Application.DTO;
using Application.Mappers;
using Domain.Entities;

namespace Application.Tests.Mappers
{
    public class MapperTests
    {
        private readonly UserMapper userMapper = new();
        private readonly ContactMapper contactMapper = new();

        [Fact]
        public void UserToModel_ShouldUseDefaults_WhenDtoFieldsAreNull()
        {
            var user = userMapper.ToModel(new UserDto());

            Assert.Equal(string.Empty, user.FirstName);
            Assert.Equal(string.Empty, user.LastName);
            Assert.Equal(string.Empty, user.Email);
            Assert.Equal(string.Empty, user.PhoneNumber);
            Assert.Equal(DateTime.MinValue, user.DateOfBirth);
        }

        [Fact]
        public void UserApplyTo_ShouldKeepId()
        {
            var user = new User { FirstName = "Old" };
            var id = user.Id;

            userMapper.ApplyTo(new UserDto { Id = Guid.NewGuid(), FirstName = "New" }, user);

            Assert.Equal("New", user.FirstName);
            Assert.Equal(id, user.Id);
        }

        [Fact]
        public void ContactToModel_ShouldUseDefaults_WhenDtoFieldsAreNull()
        {
            var contact = contactMapper.ToModel(new ContactDto());

            Assert.Equal(string.Empty, contact.FirstName);
            Assert.Equal(string.Empty, contact.Email);
            Assert.Equal(DateTime.MinValue, contact.DateOfBirth);
            Assert.Equal(Guid.Empty, contact.UserId);
        }

        [Fact]
        public void ContactApplyTo_ShouldKeepIdAndOwner()
        {
            var ownerId = Guid.NewGuid();
            var contact = new Contact { UserId = ownerId, FirstName = "Old" };
            var id = contact.Id;

            contactMapper.ApplyTo(new ContactDto { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), FirstName = "New" }, contact);

            Assert.Equal("New", contact.FirstName);
            Assert.Equal(id, contact.Id);
            Assert.Equal(ownerId, contact.UserId);
        }
    }
}
