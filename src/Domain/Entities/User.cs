using Domain.Entities.Interfaces;

namespace Domain.Entities
{
    public class User : IModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string PasswordHash { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; } = DateTime.MinValue;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;
    }
}
