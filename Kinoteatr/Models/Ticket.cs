namespace Kinoteatr.Models;

public class Ticket
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public int SeatId { get; set; }
    public int? PurchaseId { get; set; }
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; } = "reserved"; // reserved, paid, used, returned, cancelled
    
    // Навигационные свойства
    public virtual Session Session { get; set; } = null!;
    public virtual Seat Seat { get; set; } = null!;
    public virtual Purchase? Purchase { get; set; }
    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
}

