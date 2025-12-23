namespace Kinoteatr.Models;

public class Refund
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public string Status { get; set; } = "pending"; // pending, approved, rejected
    public decimal Amount { get; set; }
    
    // Навигационные свойства
    public virtual Ticket Ticket { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

