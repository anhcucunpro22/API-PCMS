namespace Printing.AppModels
{
    public class LoginResponse
    {
        public string UserMessage { get; set; }
        public string AccessToken { get; set; }
        public List<string> Facilities { get; set; }
    }
}
