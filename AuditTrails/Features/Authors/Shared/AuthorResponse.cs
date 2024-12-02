using AuditTrails.Features.Books.Shared;

namespace AuditTrails.Features.Authors.Shared;

public sealed record AuthorResponse(Guid Id, string Name, List<BookResponse> Books);
