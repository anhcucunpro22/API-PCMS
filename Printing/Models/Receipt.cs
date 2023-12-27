using Printing.AppModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Printing.Models;

public partial class Receipt
{
    [Key]
    public int ReceiptID { get; set; }

    public int? UserID { get; set; }

    public DateTime? ReceiptDate { get; set; }

    public int? ReceiptNumber { get; set; }

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

    public decimal? DepositPayment { get; set; }

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
            return AmountReceived + TaxAmount - DiscountAmount - DepositPayment;
        }
        set
        {
            // Không cần thực hiện gì trong phương thức setter
        }
    }

    public string? PaymentMethod { get; set; }

    public  ICollection<ReceiptDetail> ReceiptDetails { get; set; } = new List<ReceiptDetail>();

    [ForeignKey("UserID")]
    public Users? User { get; set; }

    public void UpdateAddUser(UpdateAddUser UpdateAddUser)
    {
        UserID = UpdateAddUser.UserID;
        PercentageDiscount = UpdateAddUser.PercentageDiscount;
        DepositPayment = UpdateAddUser.DepositPayment;
        PercentageTax = UpdateAddUser.PercentageTax;
        PaymentMethod = UpdateAddUser.PaymentMethod;
    }
}
