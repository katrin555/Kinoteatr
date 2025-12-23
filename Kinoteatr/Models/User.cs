using Microsoft.AspNetCore.Identity;

namespace Kinoteatr.Models;

public class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string Phone { get; set; } = string.Empty;
    
    // Навигационные свойства
    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
}

