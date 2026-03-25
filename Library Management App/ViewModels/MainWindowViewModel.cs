using Library_Management_App.Views;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using Library_Management_App.Views;
using Library_Management_App.ViewModels;

namespace Library_Management_App.ViewModels;

private MemberViewModel? _memberViewModel;
private MemberView? _memberView;
public partial class MainWindowViewModel : ViewModelBase
{
    private readonly string _filePath = "Data/users.json";

    private LoginViewModel _loginViewModel;
    private HomeViewModel _homeViewModel;
    private RegisterViewModel _registerViewModel;
    private LibrarianViewModel _librarianViewModel;

    private LoginView LoginView { get; }
    private HomeView HomeView { get; }
    private RegisterView RegisterView { get; }
    private LibrarianView LibrarianView { get; }

    [ObservableProperty]
    private UserControl _currentView;

    public MainWindowViewModel()
    {
        _homeViewModel = new();
        _loginViewModel = new(_filePath);
        _registerViewModel = new(_filePath);
        _librarianViewModel = new();

        LoginView = new() { DataContext = _loginViewModel };
        HomeView = new() { DataContext = _homeViewModel };
        RegisterView = new() { DataContext = _registerViewModel };
        LibrarianView = new() { DataContext = _librarianViewModel };

        CurrentView = LoginView;

        _loginViewModel.LoginSuccessful += permission =>
        {
            if (permission.Equals("librarian", StringComparison.OrdinalIgnoreCase) ||
                permission.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                CurrentView = LibrarianView;
                return;
            }

      _memberViewModel = new MemberViewModel(_loginViewModel.UserName);
      _memberView = new MemberView() { DataContext = _memberViewModel };

      _memberViewModel.LogoutRequested += () => CurrentView = LoginView;

        CurrentView = _memberView;
        };
        _loginViewModel.RegisterClicked += () => CurrentView = RegisterView;
        _registerViewModel.LoginClicked += () => CurrentView = LoginView;
        _librarianViewModel.LogoutRequested += () => CurrentView = LoginView;
    }
}
