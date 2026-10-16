using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data;

public static class DatabaseSeeder
{
    // Seeds sample data only when the database is empty (fresh database).
    public static async Task SeedDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (await context.Users.AnyAsync(cancellationToken) || await context.Contacts.AnyAsync(cancellationToken))
        {
            return;
        }

        // Placeholder value: seeded users are not meant to authenticate.
        const string placeholderPasswordHash = "SEEDED-PLACEHOLDER-NOT-A-REAL-HASH";

        var admin = new User
        {
            FirstName = "Ada",
            LastName = "Admin",
            DateOfBirth = new DateTime(1985, 3, 14),
            Email = "ada.admin@example.com",
            PhoneNumber = "+351910000001",
            PasswordHash = placeholderPasswordHash,
        };

        var user = new User
        {
            FirstName = "Ulysses",
            LastName = "User",
            DateOfBirth = new DateTime(1992, 7, 2),
            Email = "ulysses.user@example.com",
            PhoneNumber = "+351910000002",
            PasswordHash = placeholderPasswordHash,
        };

        context.Users.AddRange(admin, user);

        context.Contacts.AddRange(
            new Contact
            {
                FirstName = "Grace",
                LastName = "Hopper",
                DateOfBirth = new DateTime(1906, 12, 9),
                Email = "grace.hopper@example.com",
                PhoneNumber = "+351920000001",
                User = admin,
            },
            new Contact
            {
                FirstName = "Alan",
                LastName = "Turing",
                DateOfBirth = new DateTime(1912, 6, 23),
                Email = "alan.turing@example.com",
                PhoneNumber = "+351920000002",
                User = admin,
            },
            new Contact
            {
                FirstName = "Katherine",
                LastName = "Johnson",
                DateOfBirth = new DateTime(1918, 8, 26),
                Email = "katherine.johnson@example.com",
                PhoneNumber = "+351920000003",
                User = user,
            },
            new Contact
            {
                FirstName = "Linus",
                LastName = "Torvalds",
                DateOfBirth = new DateTime(1969, 12, 28),
                Email = "linus.torvalds@example.com",
                PhoneNumber = "+351920000004",
                User = user,
            });

        await context.SaveChangesAsync(cancellationToken);
    }
}
