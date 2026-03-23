using System;
using System.Collections.Generic;

namespace Library_Management_App.Data;

public interface IFIleBackend<T>
{
    public bool Save(T data);

    public List<T> Load();
}