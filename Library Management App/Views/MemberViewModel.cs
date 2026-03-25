using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Library_Management_App.Data;
using Library_Management_App.Models;

namespace Library_Management_App.ViewModels;
public partial class MemberViewModel : ViewModelBase
{
    private readonly GenericFileBackend<Book> _bookBackend;
    private readonly GenericFileBackend<LoanRecord> _loanBackend;

    private readonly string _currentUser;

    public event Action? LogoutRequested;
    public ObservableCollection<Book> Catalog { get; } = new();

    public ObservableCollection<LoanRecord> MyLoans { get; } = new();

    [ObservableProperty]
    private Book? _selectedBook;

    [ObservableProperty]
    private LoanRecord? _selectedLoan;

    [ObservableProperty]
    private string _searchText = "";

    [ObservableProperty]
    private string _statusMessage = "";

    public MemberViewModel(string username,
        string booksPath = "Data/books.json",
        string loansPath = "Data/loans.json")
    {
        _currentUser = username;

        _bookBackend = new GenericFileBackend<Book>(booksPath);
        _loanBackend = new GenericFileBackend<LoanRecord>(loansPath);

        LoadCatalog();
        LoadMyLoans();
    }

    [RelayCommand]
    private void LoadCatalog()
    {
        Catalog.Clear();

        List<Book> books = _bookBackend.Load();

        foreach (var book in books)
            Catalog.Add(book);
    }

    [RelayCommand]
    private void LoadMyLoans()
    {
        MyLoans.Clear();

        var loans = _loanBackend.Load()
            .Where(l => l.MemberUserName == _currentUser && !l.IsReturned);

        foreach (var loan in loans)
            MyLoans.Add(loan);
    }

    [RelayCommand]
    private void BorrowBook()
    {
        if (SelectedBook == null)
        {
            StatusMessage = "Select a book first.";
            return;
        }

        if (SelectedBook.CopiesAvailable <= 0)
        {
            StatusMessage = "Book not available.";
            return;
        }

        // decrease available copies
        SelectedBook.CopiesAvailable--;

        // create loan
        LoanRecord loan = new()
        {
            Id = Guid.NewGuid().ToString("N"),
            BookId = SelectedBook.Id,
            BookTitle = SelectedBook.Title,
            MemberUserName = _currentUser,
            BorrowedAtUtc = DateTime.UtcNow,
            DueAtUtc = DateTime.UtcNow.AddDays(21),
            IsReturned = false
        };

        List<LoanRecord> loans = _loanBackend.Load();
        loans.Add(loan);
        _loanBackend.SaveAll(loans);

        // save updated catalog
        _bookBackend.SaveAll(Catalog.ToList());

        StatusMessage = $"Borrowed '{SelectedBook.Title}'";

        LoadMyLoans();
    }
    /// Return selected loan
    [RelayCommand]
    private void ReturnBook()
    {
        if (SelectedLoan == null)
        {
            StatusMessage = "Select a loan.";
            return;
        }

        List<LoanRecord> loans = _loanBackend.Load();
        LoanRecord? loan = loans.FirstOrDefault(l => l.Id == SelectedLoan.Id);

        if (loan == null)
            return;

        loan.IsReturned = true;

        // increase book copies
        List<Book> books = _bookBackend.Load();
        Book? book = books.FirstOrDefault(b => b.Id == loan.BookId);

        if (book != null)
            book.CopiesAvailable++;

        _loanBackend.SaveAll(loans);
        _bookBackend.SaveAll(books);

        StatusMessage = "Book returned successfully.";

        LoadCatalog();
        LoadMyLoans();
    }
    /// Search/filter books by title or author
    [RelayCommand]
    private void Search()
    {
        List<Book> books = _bookBackend.Load();

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            LoadCatalog();
            return;
        }

        var filtered = books.Where(b =>
            b.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            b.Author.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        Catalog.Clear();

        foreach (var book in filtered)
            Catalog.Add(book);
    }

    [RelayCommand]
    private void Logout()
    {
        LogoutRequested?.Invoke();
    }
}