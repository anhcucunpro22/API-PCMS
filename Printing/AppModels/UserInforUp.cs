namespace Printing.AppModels
{
    public class UserInforUp
    {
        public string? FullName { get; set; }

        public string Email { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string CodeUser { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public int? OrganizationID { get; set; }
    }
}
