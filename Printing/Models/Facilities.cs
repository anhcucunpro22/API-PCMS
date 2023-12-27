using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Printing.Models;

public class Facilities
{
    [Key]
    public int FacilityID { get; set; }

    public string? FacilityName { get; set; }

    public ICollection<Services> Services { get; set; } = new List<Services>();

    public  ICollection<Photocopier> Photocopiers { get; set; } = new List<Photocopier>();

    public  ICollection<UserFacilities> UserFacilities { get; set; } = new List<UserFacilities>();

    public ICollection<InventoryOut> InventoryOuts { get; set; } = new List<InventoryOut>();
}
