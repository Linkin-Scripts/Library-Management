using System.Collections.Generic;

namespace Library_Management_App.Models;

public class User : IUser
{
    public string UserName { get; init; }
    public string Password { get; init; }
    public string Permission { get; init; } = "user";


    public Dictionary<string, string> GetUserInformation()
    {
        return new Dictionary<string, string> { 
                    {"UserName", UserName},
                    {"Password", Password},
                    {"Permission", Permission}
        };
    }
}