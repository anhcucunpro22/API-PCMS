using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Printing.Models;

public partial class Photocopier
{
    [Key]
    public int PhotocopierID { get; set; }

    public int? FacilityID { get; set; }

    public string PhotocopierName { get; set; } = null!;

    public string? Description { get; set; }

    public string? SerialNumber  { get; set; }

    public string? Location { get; set; }

    public bool? IsActive { get; set; }

    public string? Notes { get; set; }


    [ForeignKey("FacilityID")]
    public Facilities? Facility { get; set; }

    public  ICollection<ReceiptDetail> ReceiptDetails { get; set; } = new List<ReceiptDetail>();
}
