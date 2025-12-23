namespace Kinoteatr.Models;

public class Session
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public int HallId { get; set; }
    public decimal Price { get; set; }
    public DateTime DateTime { get; set; }
    public string Status { get; set; } = "active"; // active, cancelled, completed
    
    // Навигационные свойства
    public virtual Movie Movie { get; set; } = null!;
    public virtual Hall Hall { get; set; } = null!;
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}

