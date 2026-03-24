using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Library_Management_App.Data;
using Library_Management_App.Models;


namespace Library_Management_App.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    public event Action? LoginSuccessful;
    public event Action? RegisterClicked;
    private IFIleBackend<IUser> _fileBackend;


    [ObservableProperty]
    private string _userName = "";

    [ObservableProperty]
    private string _password = "";

    [RelayCommand]
    private void Login()
    {
        if (!RegisterViewModel.CheckExistingUser(_fileBackend, UserName) && CheckPassword())
            LoginSuccessful?.Invoke();
    }
    [RelayCommand]
    private void NavigateToRegister()
    {
        RegisterClicked?.Invoke();
    }

    public LoginViewModel(string filePath)
    {
        _fileBackend = new FileBackend(filePath);
    }

    private bool CheckPassword()
    {
        List<IUser> users = _fileBackend.Load();

        foreach(IUser user in users)
        {
            var userInfo = user.GetUserInformation();

            if(userInfo["UserName"] == UserName && userInfo["Password"] == Password)
                return true;
        }
        
        return false;
    }
}
