namespace AuditTrails.Database.Entities;

public class Author : IAuditableEntity
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public List<Book> Books { get; set; } = [];

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string? UpdatedBy { get; set; }
}
