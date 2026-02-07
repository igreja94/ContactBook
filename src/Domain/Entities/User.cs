using Domain.Enums;

namespace Domain.Entities
{
    public class User : Person
    {
        Guid Id { get; set; } = Guid.NewGuid();
        string PasswordHash { get; set; } = string.Empty;
        Role Role { get; set; } = Role.USER;
        List<Person> Contacts { get; set; } = [];
    }
}
