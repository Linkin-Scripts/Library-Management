using System;
using System.Collections.Generic;
using System.IO;
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
        // Load existing users
        List<IUser> users = Load();

        // Add the new user
        users.Add(user);

        // Save everything again
        return SaveAll(users);
    }

    public List<IUser> Load()
    {
        List<User> users = new();

        if (File.Exists(_filePath))
        {
            string jsonString = File.ReadAllText(_filePath);

            users = JsonSerializer.Deserialize<List<User>>(jsonString) ?? new();
        }

        return users.Cast<IUser>().ToList();
    }

    public bool SaveAll(List<IUser> users)
    {
        // Convert interface IUser to concrete class User
        List<User> concreteUsers = users.Cast<User>().ToList();

        // Convert to JSON
        string jsonString = JsonSerializer.Serialize(concreteUsers, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        // Write JSON to file
        File.WriteAllText(_filePath, jsonString);

        return true;
    }
}