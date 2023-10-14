using System.Text.Json.Serialization;

namespace api.Database.Models;

public partial class User
{
    public long Id { get; set; }

    public string Password { get; set; } = null!;

    public double Balance { get; set; }

    public string Username { get; set; } = null!;

    public bool IsAdmin { get; set; }

    [JsonIgnore]
    public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();
    [JsonIgnore]
    public virtual ICollection<Transport> Transports { get; set; } = new List<Transport>();
}
