using System;
using System.Collections.Generic;

namespace api.Database.Models;

public partial class Transport
{
    public long Id { get; set; }

    public bool CanBeRented { get; set; }

    public long IdTransportType { get; set; }

    public long IdColor { get; set; }

    public long IdModel { get; set; }

    public string Identifier { get; set; } = null!;

    public double Latitute { get; set; }

    public double Longitude { get; set; }

    public string? Description { get; set; }

    public double? MinutePrice { get; set; }

    public double? DayPrice { get; set; }

    public long IdOwner { get; set; }

    public virtual Color IdColorNavigation { get; set; } = null!;

    public virtual Model IdModelNavigation { get; set; } = null!;

    public virtual User IdOwnerNavigation { get; set; } = null!;

    public virtual TransportType IdTransportTypeNavigation { get; set; } = null!;

    public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();
}
