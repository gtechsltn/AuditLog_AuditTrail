namespace AuditTrails.Models;

public record BookDto
{
    public required Guid Id { get; init; }
    
    public required string Title { get; init; }
    
    public required int Year { get; init; }
    
    public required AuthorDto Author { get; set; }
}