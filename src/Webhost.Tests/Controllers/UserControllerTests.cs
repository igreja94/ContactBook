using System.Net;
using System.Net.Http.Json;
using Application.DTO;
using Webhost.Tests.Extensions;
using Webhost.Tests.Fixtures;

namespace Webhost.Tests.Controllers;

[Collection(WebhostCollectionDefinition.Name)]
public class UserControllerTests(WebhostFixture fixture)
{
    private readonly HttpClient client = fixture.Client;

    [Fact]
    public async Task GetAll_ReturnsSeededUsers()
    {
        var response = await client.GetAsync("/api/User");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        Assert.Contains(users!, u => u.Email == "ada.admin@example.com" && u.FirstName == "Ada");
        Assert.Contains(users!, u => u.Email == "ulysses.user@example.com" && u.FirstName == "Ulysses");
    }

    [Fact]
    public async Task GetById_ReturnsSeededUser()
    {
        var seeded = await client.GetSeededUserAsync("ada.admin@example.com");

        var response = await client.GetAsync($"/api/User/{seeded.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.Equal("Ada", user!.FirstName);
        Assert.Equal("Admin", user.LastName);
        Assert.Equal(new DateTime(1985, 3, 14), user.DateOfBirth);
        Assert.Equal("+351910000001", user.PhoneNumber);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var response = await client.GetAsync($"/api/User/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenIdIsNotAGuid()
    {
        var response = await client.GetAsync("/api/User/not-a-guid");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreatedWithServerAssignedId()
    {
        var clientSuppliedId = Guid.NewGuid();

        var response = await client.PostAsJsonAsync("/api/User", new UserDto
        {
            Id = clientSuppliedId,
            FirstName = "Grace",
            LastName = "New",
            DateOfBirth = new DateTime(2000, 2, 2),
            Email = "grace.new@example.com",
            PhoneNumber = "+351911111111",
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotEqual(clientSuppliedId, created!.Id);
        Assert.EndsWith($"/api/User/{created.Id}", response.Headers.Location!.ToString());

        var fetched = await client.GetFromJsonAsync<UserDto>($"/api/User/{created.Id}");
        Assert.Equal("Grace", fetched!.FirstName);
        Assert.Equal("grace.new@example.com", fetched.Email);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenRequiredFieldsAreMissing()
    {
        var response = await client.PostAsJsonAsync("/api/User", new UserDto { LastName = "NoFirstName" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenFieldExceedsMaxLength()
    {
        var response = await client.PostAsJsonAsync("/api/User", new UserDto
        {
            FirstName = new string('a', 101),
            LastName = "Long",
            DateOfBirth = new DateTime(2000, 1, 1),
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_ModifiesUser()
    {
        var user = await client.CreateUserAsync("Before");
        user.FirstName = "After";
        user.PhoneNumber = "+351922222222";

        var response = await client.PutAsJsonAsync("/api/User", user);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var fetched = await client.GetFromJsonAsync<UserDto>($"/api/User/{user.Id}");
        Assert.Equal("After", fetched!.FirstName);
        Assert.Equal("+351922222222", fetched.PhoneNumber);
        Assert.Equal(user.Email, fetched.Email);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var response = await client.PutAsJsonAsync("/api/User", new UserDto
        {
            Id = Guid.NewGuid(),
            FirstName = "Ghost",
            LastName = "User",
            DateOfBirth = new DateTime(1990, 1, 1),
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenIdIsMissing()
    {
        var response = await client.PutAsJsonAsync("/api/User", new UserDto
        {
            FirstName = "No",
            LastName = "Id",
            DateOfBirth = new DateTime(1990, 1, 1),
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesUserAndTheirContacts()
    {
        var user = await client.CreateUserAsync();
        var contact = await client.CreateContactAsync(user.Id!.Value);

        var response = await client.DeleteAsync($"/api/User/{user.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/User/{user.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/Contact/{contact.Id}")).StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var response = await client.DeleteAsync($"/api/User/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
