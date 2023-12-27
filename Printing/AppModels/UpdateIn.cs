namespace Printing.AppModels
{
    public class UpdateIn
    {
        public int? WarehouseID { get; set; }

        public int? SupplierID { get; set; }

        public decimal? PercentageDiscount { get; set; }

        public decimal? PercentageTax { get; set; }

        public string? PaymentMethod { get; set; }
    }
}
