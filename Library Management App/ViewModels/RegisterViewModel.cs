using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Library_Management_App.Data;
using Library_Management_App.Models;

namespace Library_Management_App.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    private IFIleBackend<IUser> _fileBackend;
    public event Action? LoginClicked;

    [ObservableProperty]
    private string _userName = "";

    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private string _confirmPassword = "";

    public RegisterViewModel(string filePath)
    {
        _fileBackend = new FileBackend(filePath);
    }

    [RelayCommand]
    private void Register()
    {
        if (IsConfirmPassword() && UserName != "" && CheckExistingUser(_fileBackend, UserName))
        {
            LoginClicked?.Invoke();
            SaveUser();
        }
    }

    [RelayCommand]
    private void NavigateToLogin()
    {
        LoginClicked?.Invoke();
    }

    private bool SaveUser()
    {
        _fileBackend.Save(new User(){ UserName = UserName, Password = Password });

        return true;
    }

    static public bool CheckExistingUser(IFIleBackend<IUser> fIleBackend, String username)
    {
        List<IUser> users = fIleBackend.Load();
    
        foreach(IUser user in users)
        {
            var userInfo = user.GetUserInformation();
            if(userInfo["UserName"] == username)
            {
                return false;
            }
        }

        return true;
    }

    private bool IsConfirmPassword()
    {
        if (Password == ConfirmPassword && Password != "")
            return true;
        else
            return false;
    }

}
