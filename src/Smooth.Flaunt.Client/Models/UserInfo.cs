namespace Smooth.Flaunt.Models;

#nullable disable

public class UserInfo
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public string PreferredUsername { get; set; }
    public string SubjectId { get; set; } // Typically the unique identifier for the user
    public IEnumerable<string> Roles { get; set; }
    public IDictionary<string, string> Claims { get; set; }
}
