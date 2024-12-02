using AuditTrails.Database;
using AuditTrails.Database.Entities;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace AuditTrails.Services;

public static class DatabaseSeedService
{
    public static async Task SeedAsync(ApplicationDbContext dbContext)
    {
	    await dbContext.Database.MigrateAsync();

	    if (await dbContext.Users.AnyAsync())
	    {
		    return;
	    }

        var users = GetUsers(3);
        var authors = GetAuthors(3);

        var books = GetBooks(20, authors);

        await dbContext.Authors.AddRangeAsync(authors);
        await dbContext.Books.AddRangeAsync(books);
        await dbContext.Users.AddRangeAsync(users);

        await dbContext.SaveChangesAsync();
    }

    private static List<User> GetUsers(int count)
    {
        return new Faker<User>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.Email, f => f.Person.Email)
            .Generate(count);
    }

    private static List<Author> GetAuthors(int count)
    {
        return new Faker<Author>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .Generate(count);
    }

    private static List<Book> GetBooks(int count, List<Author> authors)
    {
        return new Faker<Book>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.Title, f => f.Commerce.Product())
            .RuleFor(x => x.Year, f => f.Random.Int(2018, 2025))
            .RuleFor(x => x.Author, f => f.PickRandom(authors))
            .Generate(count);
    }
}
