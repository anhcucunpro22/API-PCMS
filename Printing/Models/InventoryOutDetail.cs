using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Printing.Models;

public  class InventoryOutDetail
{
    [Key]
    public int InventoryOutDeID { get; set; }

    public int? InventoryOutID { get; set; }

    public int? MaterialID { get; set; }

    public int? UnitID { get; set; }

    public int? Quantity { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? CreatedBy { get; set; } 

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? Description { get; set; }

    [ForeignKey("InventoryOutID")]
    public InventoryOut? InventoryOut { get; set; }

    [ForeignKey("MaterialID")]
    public Material? Material { get; set; }

    [ForeignKey("UnitID")]
    public UnitOfMeasure? Unit { get; set; }
}
