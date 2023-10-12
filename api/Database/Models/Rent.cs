using System;
using System.Collections.Generic;

namespace api.Database.Models;

public partial class Rent
{
    public long Id { get; set; }

    public DateTime TimeStart { get; set; }

    public DateTime? TimeEnd { get; set; }

    public long IdTransport { get; set; }

    public long IdUser { get; set; }

    public double PriceOfUnit { get; set; }

    public long PriceType { get; set; }

    public double? FinalPrice { get; set; }

    public virtual Transport IdTransportNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual RentType PriceTypeNavigation { get; set; } = null!;
}
