namespace Kinoteatr.Models;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int Duration { get; set; } // в минутах
    public string AgeRestriction { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public string ProductionCountry { get; set; } = string.Empty;
    public string? Poster { get; set; }
    
    // Навигационные свойства
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}

