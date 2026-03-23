using Library_Management_App.Views;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Library_Management_App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private LoginView LoginView {get;} = new() { DataContext= new LoginViewModel() };
    private HomeView HomeView {get;} = new() { DataContext= new HomeViewModel() };

    [ObservableProperty]
    private UserControl _currentView;

    public MainWindowViewModel()
    {
        CurrentView = LoginView;
    }

    public void NextView()
    {
        if (CurrentView == LoginView)
        {
            CurrentView = HomeView;
        }
        else
        {
            CurrentView = LoginView;
        }
 
    }

}
