using System.Net.Http.Json;
using Application.DTO;

namespace Webhost.Tests.Extensions;

internal static class ApiClientExtensions
{
    public static async Task<UserDto> CreateUserAsync(this HttpClient client, string firstName = "Temp")
    {
        var response = await client.PostAsJsonAsync("/api/User", new UserDto
        {
            FirstName = firstName,
            LastName = "User",
            DateOfBirth = new DateTime(1990, 1, 1),
            Email = $"{Guid.NewGuid():N}@example.com",
            PhoneNumber = "+351900000000"
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<UserDto>())!;
    }

    public static async Task<ContactDto> CreateContactAsync(this HttpClient client, Guid userId, string firstName = "Temp")
    {
        var response = await client.PostAsJsonAsync("/api/Contact", new ContactDto
        {
            FirstName = firstName,
            LastName = "Contact",
            DateOfBirth = new DateTime(1980, 5, 5),
            Email = $"{Guid.NewGuid():N}@example.com",
            PhoneNumber = "+351900000001",
            UserId = userId
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ContactDto>())!;
    }

    public static async Task<UserDto> GetSeededUserAsync(this HttpClient client, string email)
    {
        var users = await client.GetFromJsonAsync<List<UserDto>>("/api/User");
        return users!.Single(u => u.Email == email);
    }
}
