using AuditTrails.Database;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace AuditTrails.Features.Users;

public sealed record UpdateUserRequest(Guid Id, string Email);

public class UpdateUserEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapPut("/api/users/{id}", Handle);
	}

	private static async Task<IResult> Handle(
		[FromRoute] Guid id,
		[FromBody] UpdateUserRequest request,
		ApplicationDbContext context,
		CancellationToken cancellationToken)
	{
		var user = await context.Users.FindAsync([id], cancellationToken);
		if (user is null)
		{
			return Results.NotFound();
		}

		user.Email = request.Email;

		await context.SaveChangesAsync(cancellationToken);

		return Results.NoContent();
	}
}

