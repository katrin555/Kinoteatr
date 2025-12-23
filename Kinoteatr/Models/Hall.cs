namespace Kinoteatr.Models;

public class Hall
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public bool Supports3D { get; set; }
    
    // Навигационные свойства
    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}

