using AuditTrails.Database;
using AuditTrails.Features.Books.Shared;
using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuditTrails.Features.Books;

public class GetBookByIdEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapGet("/api/books/{id}", Handle);
	}

	private static async Task<IResult> Handle(
		[FromRoute] Guid id,
		ApplicationDbContext context,
		CancellationToken cancellationToken)
	{
		var book = await context.Books
			.Include(b => b.Author)
			.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

		if (book is null)
		{
			return Results.NotFound();
		}

		var response = new BookResponse(book.Id, book.Title, book.Year, book.AuthorId);
		return Results.Ok(response);
	}
}

