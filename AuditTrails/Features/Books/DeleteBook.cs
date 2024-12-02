using AuditTrails.Database;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace AuditTrails.Features.Books;

public class DeleteBookEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapDelete("/api/books/{id}", Handle);
	}

	private static async Task<IResult> Handle(
		[FromRoute] Guid id,
		ApplicationDbContext context,
		CancellationToken cancellationToken)
	{
		var book = await context.Books.FindAsync([id], cancellationToken);
		if (book is null)
		{
			return Results.NotFound();
		}

		context.Books.Remove(book);
		await context.SaveChangesAsync(cancellationToken);

		return Results.NoContent();
	}
}

