using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Library_Management_App.Data;
using Library_Management_App.Models;


namespace Library_Management_App.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    public event Action? LoginSuccessful;

    private IFIleBackend<IUser> _fileBackend;


    [ObservableProperty]
    private string _userName = "";

    [ObservableProperty]
    private string _password = "";

    [RelayCommand]
    private void Login()
    {
        LoginSuccessful.Invoke();

        SaveUser();
    }

    public LoginViewModel(string filePath)
    {
        _fileBackend = new FileBackend(filePath);
    }

    private bool SaveUser()
    {
        _fileBackend.Save(new User(){ UserName = UserName, Password = Password });

        return true;
    }
}
