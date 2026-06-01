using ONENET.Domain.Common;
using ONENET.Domain.Enums;

namespace ONENET.Domain.Entities;

public class Book : BaseEntity
{
    public string BookCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public int PublishYear { get; set; }
    public int Quantity { get; private set; }
    public string? ShelfLocation { get; set; }
    public BookStatus Status { get; private set; } = BookStatus.OutOfStock;

    public Book()
    {
    }

    public Book(string bookCode, string title, string author, string category, string publisher, int publishYear, int quantity, string? shelfLocation)
    {
        BookCode = bookCode;
        Title = title;
        Author = author;
        Category = category;
        Publisher = publisher;
        PublishYear = publishYear;
        ShelfLocation = shelfLocation;
        UpdateStock(quantity);
    }

    public void UpdateStock(int newQuantity)
    {
        if (newQuantity < 0)
        {
            throw new ArgumentException("Quantity cannot be negative.");
        }

        Quantity = newQuantity;
        Status = newQuantity == 0 ? BookStatus.OutOfStock : BookStatus.Available;
    }
}
