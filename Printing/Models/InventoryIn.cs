using Printing.AppModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Printing.Models;

public partial class InventoryIn
{
    [Key]
    public int InventoryInID { get; set; }

    public int? WarehouseID { get; set; }

    public int? SupplierID { get; set; }

    public DateTime? InDate { get; set; }

    public int? InNumber { get; set; }

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

    public  ICollection<InventoryInDetail> InventoryInDetails { get; set; } = new List<InventoryInDetail>();

    [ForeignKey("SupplierID")]
    public  Suppliers? Supplier { get; set; }

    [ForeignKey("WarehouseID")]
    public  Warehouses? Warehouse { get; set; }


    public void UpdateIn(UpdateIn UpdateIn)
    {
        WarehouseID = UpdateIn.WarehouseID;
        SupplierID = UpdateIn.SupplierID;
        PercentageDiscount = UpdateIn.PercentageDiscount;
        PercentageTax = UpdateIn.PercentageTax;
        PaymentMethod = UpdateIn.PaymentMethod;
    }
}
