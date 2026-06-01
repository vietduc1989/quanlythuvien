using System;
using ONENET.Domain.Enums;

namespace ONENET.Application.Books.DTOs;

public class BookDto
{
    public Guid Id { get; set; }
    public string BookCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public int PublishYear { get; set; }
    public int Quantity { get; set; }
    public string? ShelfLocation { get; set; }
    public string Status { get; set; } = string.Empty;
}
