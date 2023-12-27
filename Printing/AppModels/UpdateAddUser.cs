namespace Printing.AppModels
{
    public class UpdateAddUser
    {
        public int? UserID { get; set; }
        public decimal? PercentageDiscount { get; set; }

        public decimal? DepositPayment { get; set; }

        public decimal? PercentageTax { get; set; }

        public string? PaymentMethod { get; set; }
    }
}
