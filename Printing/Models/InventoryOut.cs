using Printing.AppModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Printing.Models;

public class InventoryOut
{
    [Key]
    public int InventoryOutID { get; set; }

    public int? WarehouseID { get; set; }

    public int? FacilityID { get; set; }

    public DateTime? OutDate { get; set; }

    public int? OutNumber { get; set; }

    public decimal? AmountReceived { get; set; }

    public decimal? PercentageDiscount { get; set; }

    public decimal? DiscountAmount
    {
        get
        {
            return (AmountReceived * PercentageDiscount) / 100;
        }
        set
        {

        }
    }

    public decimal? PercentageTax { get; set; }

    public decimal? TaxAmount
    {
        get
        {

            return (AmountReceived * PercentageTax) / 100;
        }
        set
        {
            // Không cần thực hiện gì trong phương thức setter
        }
    }

    public decimal? TotalAmount
    {
        get
        {
            return AmountReceived + TaxAmount - DiscountAmount;
        }
        set
        {
            // Không cần thực hiện gì trong phương thức setter
        }
    }
    public string? PaymentMethod { get; set; }

    public  ICollection<InventoryOutDetail> InventoryOutDetails { get; set; } = new List<InventoryOutDetail>();

    [ForeignKey("FacilityID")]
    public Facilities? Facility { get; set; }

    [ForeignKey("WarehouseID")]
    public  Warehouses? Warehouse { get; set; }

    public void UpdateOut(UpdateOut UpdateOut)
    {
        WarehouseID = UpdateOut.WarehouseID;
        FacilityID = UpdateOut.FacilityID;
        PercentageDiscount = UpdateOut.PercentageDiscount;
        PercentageTax = UpdateOut.PercentageTax;
        PaymentMethod = UpdateOut.PaymentMethod;
    }
}
