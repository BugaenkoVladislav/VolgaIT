using System.Text.Json.Serialization;

namespace api.Database.Models;

public partial class Transport
{
    public long Id { get; set; }

    public bool CanBeRented { get; set; }

    public long IdTransportType { get; set; }

    public long IdColor { get; set; }

    public long IdModel { get; set; }

    public string Identifier { get; set; } = null!;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string? Description { get; set; }

    public double? MinutePrice { get; set; }

    public double? DayPrice { get; set; }

    public long IdOwner { get; set; }
    [JsonIgnore]
    public virtual Color IdColorNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual Model IdModelNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual User IdOwnerNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual TransportType IdTransportTypeNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();
}
