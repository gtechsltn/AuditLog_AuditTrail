using AuditTrails.Database;
using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuditTrails.Features.Books;

public sealed record UpdateBookRequest(Guid Id, string Title, int Year, Guid AuthorId);

public class UpdateBookEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapPut("/api/books/{id}", Handle);
	}

	private static async Task<IResult> Handle(
		[FromRoute] Guid id,
		[FromBody] UpdateBookRequest request,
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

		var author = await context.Authors.FindAsync([request.AuthorId], cancellationToken);
		if (author is null)
		{
			return Results.BadRequest("Author not found");
		}

		book.Title = request.Title;
		book.Year = request.Year;
		book.AuthorId = request.AuthorId;
		book.Author = author;

		await context.SaveChangesAsync(cancellationToken);

		return Results.NoContent();
	}
}
