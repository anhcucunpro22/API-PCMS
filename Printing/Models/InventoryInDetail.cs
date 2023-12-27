using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Printing.Models;

public partial class InventoryInDetail
{
    [Key]
    public int InventoryInDeID { get; set; }

    public int InventoryInID { get; set; }

    public int MaterialID { get; set; }

    public int UnitID { get; set; }

    public int? Quantity { get; set; }

    public decimal? FinalPrice { get; set; }

    public string? CreatedBy { get; set; } 

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? Description { get; set; }


    [ForeignKey("InventoryInID")]
    public InventoryIn? InventoryIn { get; set; } 

    [ForeignKey("MaterialID")]
    public Material? Material { get; set; } 

    [ForeignKey("UnitID")]
    public UnitOfMeasure? Unit { get; set; } 
}
