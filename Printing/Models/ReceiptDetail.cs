using Printing.AppModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Printing.Models;

public class ReceiptDetail
{
    [Key]
    public int ReceiptDeID { get; set; }

    public int? Quantity { get; set; }

    public int? QuantitySets { get; set; }

    public decimal? FinalPrice { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? Description { get; set; }

    public int ServiceID { get; set; }
    public int ReceiptID { get; set; }
    public int PhotocopierID { get; set; }

    [ForeignKey("PhotocopierID")]
    public Photocopier? Photocopier { get; set; }

    [ForeignKey("ReceiptID")]
    public Receipt? Receipt { get; set; }

    [ForeignKey("ServiceID")]
    public Services? Services { get; set; }
}

