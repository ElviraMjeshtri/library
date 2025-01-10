using Web.API.DTOs;

namespace Web.API.Models.DTOs;

public class UpdateBookDto : CreateBookDto
{
    public string Isbn { get; set; } = default!;
}