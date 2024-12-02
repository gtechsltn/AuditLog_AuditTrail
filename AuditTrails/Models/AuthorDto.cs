namespace AuditTrails.Models;

public record AuthorDto
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required List<BookDto> Books { get; init; } = new();
}
