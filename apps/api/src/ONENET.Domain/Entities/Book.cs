using System;
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
    public int Quantity { get; set; }
    public string? ShelfLocation { get; set; }
    public BookStatus Status { get; set; }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity < 0)
        {
            throw new ArgumentException("Số lượng tồn kho không được nhỏ hơn 0.");
        }

        Quantity = newQuantity;
        Status = newQuantity > 0 ? BookStatus.Available : BookStatus.OutOfStock;
    }
}
