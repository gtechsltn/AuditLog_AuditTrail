using AuditTrails.Database;
using AuditTrails.Database.Entities;
using AuditTrails.Models;
using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuditTrails.Features.Authors;

public sealed record UpdateAuthorRequest(Guid Id, string Name);

public class UpdateAuthorEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapPut("/api/authors/{id}", Handle);
	}

	private static async Task<IResult> Handle(
		[FromRoute] Guid id,
		[FromBody] UpdateAuthorRequest request,
		ApplicationDbContext context,
		CancellationToken cancellationToken)
	{
		var author = await context.Authors
			.Include(a => a.Books)
			.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

		if (author is null)
		{
			return Results.NotFound();
		}

		author.Name = request.Name;
		await context.SaveChangesAsync(cancellationToken);

		return Results.NoContent();
	}
}
