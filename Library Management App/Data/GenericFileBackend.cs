using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Library_Management_App.Data;

public class GenericFileBackend<T> : IFIleBackend<T>
{
    private readonly string _filePath;

    public GenericFileBackend(string filePath)
    {
        _filePath = filePath;
    }

    public bool Save(T data)
    {
        List<T> allItems = Load();
        allItems.Add(data);
        Persist(allItems);
        return true;
    }

    public List<T> Load()
    {
        if (!File.Exists(_filePath))
            return new List<T>();

        string jsonString = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<T>>(jsonString) ?? new List<T>();
    }

    public bool SaveAll(List<T> allItems)
    {
        Persist(allItems);
        return true;
    }

    private void Persist(List<T> allItems)
    {
        string jsonString = JsonSerializer.Serialize(allItems);
        File.WriteAllText(_filePath, jsonString);
    }
}