using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using Library_Management_App.Models;


namespace Library_Management_App.Data;

public class FileBackend : IFIleBackend<IUser>
{
    private string _filePath;

    public FileBackend(string filePath)
    {
        _filePath = filePath;
    }

    public bool Save(IUser user)
    {
        List<IUser> users = new();        


        if(File.Exists(_filePath))
        {
            users = Load();        
        }

        users.Add(user);

        // We have to cast to Users (concrete class) since we cant serialize inconcrete properties
        List<User> concreteUsers = users.Cast<User>().ToList();

        string jsonString = JsonSerializer.Serialize(concreteUsers);

        File.WriteAllText(_filePath, jsonString);

        return true;
    }

    public List<IUser> Load()
    {
        List<User> users = new();

        if(File.Exists(_filePath))
        {
            string jsonString = File.ReadAllText(_filePath);

            users = JsonSerializer.Deserialize<List<User>>(jsonString) ?? new();
        }

        return users.Cast<IUser>().ToList();
    }
}