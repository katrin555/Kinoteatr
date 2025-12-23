namespace Kinoteatr.Models;

public class Seat
{
    public int Id { get; set; }
    public int HallId { get; set; }
    public int Row { get; set; }
    public int SeatNumber { get; set; }
    public string Type { get; set; } = string.Empty; // обычное, VIP, и т.д.
    public string Status { get; set; } = "available"; // available, broken, etc.
    
    // Навигационные свойства
    public virtual Hall Hall { get; set; } = null!;
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}

