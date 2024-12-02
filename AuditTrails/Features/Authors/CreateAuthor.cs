using AuditTrails.Database;
using AuditTrails.Database.Entities;
using AuditTrails.Features.Authors.Shared;
using AuditTrails.Features.Books.Shared;
using AuditTrails.Models;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace AuditTrails.Features.Authors;

public sealed record CreateAuthorRequest(string Name);

public class CreateAuthorEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapPost("/api/authors", Handle);
	}

	private static async Task<IResult> Handle(
		[FromBody] CreateAuthorRequest request,
		ApplicationDbContext context,
		CancellationToken cancellationToken)
	{
		var author = new Author
		{
			Id = Guid.NewGuid(),
			Name = request.Name
		};

		context.Authors.Add(author);
		await context.SaveChangesAsync(cancellationToken);

		var response = new AuthorResponse(author.Id, author.Name, []);

		return Results.Created($"/api/authors/{author.Id}", response);
	}
}
