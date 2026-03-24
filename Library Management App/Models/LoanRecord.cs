using System;

namespace Library_Management_App.Models;

public class LoanRecord
{
    public string Id { get; set; } = string.Empty;
    public string BookId { get; set; } = string.Empty;
    public string BookTitle { get; set; } = string.Empty;
    public string MemberUserName { get; set; } = string.Empty;
    public DateTime BorrowedAtUtc { get; set; }
    public DateTime DueAtUtc { get; set; }
    public bool IsReturned { get; set; }
}