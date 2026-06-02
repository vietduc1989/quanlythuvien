using System;
using System.Collections.Generic;

namespace ONENET.Application.Loans.DTOs;

public class LoanDto
{
    public Guid LoanId { get; set; }
    public Guid ReaderId { get; set; }
    public string ReaderName { get; set; } = string.Empty;
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public string LoanStatus { get; set; } = string.Empty;
    public List<LoanDetailDto> Details { get; set; } = new();
}

public class LoanDetailDto
{
    public Guid LoanDetailId { get; set; }
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public DateTime? ActualReturnDate { get; set; }
    public string DetailStatus { get; set; } = string.Empty;
    public bool IsOverdue { get; set; }
}
