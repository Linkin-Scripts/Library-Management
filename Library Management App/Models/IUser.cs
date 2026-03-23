using System.Collections.Generic;

namespace Library_Management_App.Models;

public interface IUser
{
    public Dictionary<string, string> GetUserInformation();
}