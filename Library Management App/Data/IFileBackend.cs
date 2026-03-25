using System;
using System.Collections.Generic;

namespace Library_Management_App.Data;

public interface IFIleBackend<T>
{
    bool Save(T data);
    List<T> Load();
    // Saves an entire list of objects to storage
    bool SaveAll(List<T> allItems);
}