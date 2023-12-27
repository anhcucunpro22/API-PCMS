namespace Printing.AppModels
{
    public class UpdateOut
    {
        public int? WarehouseID { get; set; }

        public int? FacilityID { get; set; }

        public decimal? PercentageDiscount { get; set; }

        public decimal? PercentageTax { get; set; }

        public string? PaymentMethod { get; set; }
    }
}
