using AuditTrails.Database.Entities;
using AuditTrails.Models;

namespace AuditTrails.Mapping;

public static class MappingExtensions
{
    public static BookDto MapToBookDto(this Book entity)
        => new()
        {
            Id = entity.Id,
            Title = entity.Title,
            Year = entity.Year,
            Author = new AuthorDto
            {
                Id = entity.Author.Id,
                Name = entity.Author.Name,
                Books = []
            }
        };
    
    public static AuthorDto MapToAuthorDto(this Author entity)
        => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Books = entity.Books.Select(x => new BookDto
            {
                Id = x.Id,
                Title = x.Title,
                Year = x.Year,
                Author = null!
            }).ToList()
        };
}