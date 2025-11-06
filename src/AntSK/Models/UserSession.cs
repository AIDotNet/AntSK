namespace AntSK.Models
{
    public class UserSession
    {
        public string UserName { get; set; }
        public string Role { get; set; }
        public List<string>? Roles { get; set; }
        public List<string>? Permissions { get; set; }
    }
}
