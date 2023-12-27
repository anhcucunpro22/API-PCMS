namespace Printing.AppModels
{
    public class PhotocopierCreationRequest
    {
        public string PhotocopierName { get; set; }
        public string Description { get; set; }
        public string SerialNumber { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
        public string Notes { get; set; }
    }
}
