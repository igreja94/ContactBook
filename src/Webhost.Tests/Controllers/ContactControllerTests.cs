using System.Net;
using System.Net.Http.Json;
using Application.DTO;
using Webhost.Tests.Extensions;
using Webhost.Tests.Fixtures;

namespace Webhost.Tests.Controllers;

[Collection(WebhostCollectionDefinition.Name)]
public class ContactControllerTests(WebhostFixture fixture)
{
    private readonly HttpClient client = fixture.Client;

    [Fact]
    public async Task GetAll_ReturnsSeededContacts()
    {
        var response = await client.GetAsync("/api/Contact");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var contacts = await response.Content.ReadFromJsonAsync<List<ContactDto>>();
        foreach (var email in new[]
        {
            "grace.hopper@example.com", "alan.turing@example.com",
            "katherine.johnson@example.com", "linus.torvalds@example.com",
        })
        {
            Assert.Contains(contacts!, c => c.Email == email);
        }
    }

    [Fact]
    public async Task GetById_ReturnsSeededContact()
    {
        var contacts = await client.GetFromJsonAsync<List<ContactDto>>("/api/Contact");
        var grace = contacts!.Single(c => c.Email == "grace.hopper@example.com");

        var response = await client.GetAsync($"/api/Contact/{grace.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var contact = await response.Content.ReadFromJsonAsync<ContactDto>();
        Assert.Equal("Grace", contact!.FirstName);
        Assert.Equal("Hopper", contact.LastName);
        Assert.Equal(new DateTime(1906, 12, 9), contact.DateOfBirth);
        Assert.Equal("+351920000001", contact.PhoneNumber);
        var ada = await client.GetSeededUserAsync("ada.admin@example.com");
        Assert.Equal(ada.Id, contact.UserId);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenContactDoesNotExist()
    {
        var response = await client.GetAsync($"/api/Contact/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetByUser_ReturnsOnlyThatUsersContacts()
    {
        var ulysses = await client.GetSeededUserAsync("ulysses.user@example.com");

        var response = await client.GetAsync($"/api/Contact/user/{ulysses.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var contacts = await response.Content.ReadFromJsonAsync<List<ContactDto>>();
        Assert.Equal(
            ["katherine.johnson@example.com", "linus.torvalds@example.com"],
            contacts!.Select(c => c.Email ?? string.Empty).Order().ToArray());
        Assert.All(contacts!, c => Assert.Equal(ulysses.Id, c.UserId));
    }

    [Fact]
    public async Task GetByUser_ReturnsEmptyList_WhenUserHasNoContacts()
    {
        var user = await client.CreateUserAsync();

        var contacts = await client.GetFromJsonAsync<List<ContactDto>>($"/api/Contact/user/{user.Id}");

        Assert.Empty(contacts!);
    }

    [Fact]
    public async Task GetByUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var response = await client.GetAsync($"/api/Contact/user/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAndLinksContactToUser()
    {
        var user = await client.CreateUserAsync();

        var response = await client.PostAsJsonAsync("/api/Contact", new ContactDto
        {
            FirstName = "Margaret",
            LastName = "Hamilton",
            DateOfBirth = new DateTime(1936, 8, 17),
            Email = "margaret.hamilton@example.com",
            PhoneNumber = "+351933333333",
            UserId = user.Id,
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<ContactDto>();
        Assert.NotEqual(Guid.Empty, created!.Id);
        Assert.EndsWith($"/api/Contact/{created.Id}", response.Headers.Location!.ToString());

        var contacts = await client.GetFromJsonAsync<List<ContactDto>>($"/api/Contact/user/{user.Id}");
        var single = Assert.Single(contacts!);
        Assert.Equal(created.Id, single.Id);
        Assert.Equal("Margaret", single.FirstName);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenUserIdIsMissing()
    {
        var response = await client.PostAsJsonAsync("/api/Contact", new ContactDto
        {
            FirstName = "No",
            LastName = "Owner",
            DateOfBirth = new DateTime(1990, 1, 1),
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenUserDoesNotExist()
    {
        var response = await client.PostAsJsonAsync("/api/Contact", new ContactDto
        {
            FirstName = "Orphan",
            LastName = "Contact",
            DateOfBirth = new DateTime(1990, 1, 1),
            UserId = Guid.NewGuid(),
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenRequiredFieldsAreMissing()
    {
        var user = await client.CreateUserAsync();

        var response = await client.PostAsJsonAsync("/api/Contact", new ContactDto { UserId = user.Id });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_ModifiesContactAndKeepsOwner()
    {
        var owner = await client.CreateUserAsync();
        var otherUser = await client.CreateUserAsync();
        var contact = await client.CreateContactAsync(owner.Id!.Value, "Before");
        contact.FirstName = "After";
        contact.UserId = otherUser.Id; // Ownership cannot be changed through an update.

        var response = await client.PutAsJsonAsync("/api/Contact", contact);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var fetched = await client.GetFromJsonAsync<ContactDto>($"/api/Contact/{contact.Id}");
        Assert.Equal("After", fetched!.FirstName);
        Assert.Equal(owner.Id, fetched.UserId);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenContactDoesNotExist()
    {
        var response = await client.PutAsJsonAsync("/api/Contact", new ContactDto
        {
            Id = Guid.NewGuid(),
            FirstName = "Ghost",
            LastName = "Contact",
            DateOfBirth = new DateTime(1990, 1, 1),
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesContact()
    {
        var user = await client.CreateUserAsync();
        var contact = await client.CreateContactAsync(user.Id!.Value);

        var response = await client.DeleteAsync($"/api/Contact/{contact.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/Contact/{contact.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/User/{user.Id}")).StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenContactDoesNotExist()
    {
        var response = await client.DeleteAsync($"/api/Contact/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteByUser_RemovesOnlyTheUsersSelectedContacts()
    {
        var user = await client.CreateUserAsync();
        var otherUser = await client.CreateUserAsync();
        var first = await client.CreateContactAsync(user.Id!.Value, "First");
        var second = await client.CreateContactAsync(user.Id!.Value, "Second");
        var foreign = await client.CreateContactAsync(otherUser.Id!.Value, "Foreign");

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/Contact/user/{user.Id}")
        {
            Content = JsonContent.Create(new[] { first.Id, foreign.Id }),
        };
        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/Contact/{first.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/Contact/{second.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/Contact/{foreign.Id}")).StatusCode);
    }

    [Fact]
    public async Task DeleteByUser_ReturnsBadRequest_WhenNoContactIdsAreProvided()
    {
        var user = await client.CreateUserAsync();

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/Contact/user/{user.Id}")
        {
            Content = JsonContent.Create(Array.Empty<string>()),
        };
        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteByUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/Contact/user/{Guid.NewGuid()}")
        {
            Content = JsonContent.Create(new[] { Guid.NewGuid() }),
        };
        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
