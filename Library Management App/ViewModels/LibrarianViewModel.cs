using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Library_Management_App.Data;
using Library_Management_App.Models;

namespace Library_Management_App.ViewModels;

public partial class LibrarianViewModel : ViewModelBase
{
    private readonly GenericFileBackend<Book> _bookBackend;
    private readonly GenericFileBackend<LoanRecord> _loanBackend;
    public event Action? LogoutRequested;

    public ObservableCollection<Book> Catalog { get; } = new();
    public ObservableCollection<LoanRecord> ActiveLoans { get; } = new();

    [ObservableProperty]
    private Book? _selectedBook;

    [ObservableProperty]
    private string _bookTitle = string.Empty;

    [ObservableProperty]
    private string _bookAuthor = string.Empty;

    [ObservableProperty]
    private string _bookIsbn = string.Empty;

    [ObservableProperty]
    private int _bookCopiesAvailable = 1;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public LibrarianViewModel(string booksFilePath = "Data/books.json", string loansFilePath = "Data/loans.json")
    {
        _bookBackend = new GenericFileBackend<Book>(booksFilePath);
        _loanBackend = new GenericFileBackend<LoanRecord>(loansFilePath);

        LoadCatalog();
        LoadActiveLoans();
    }

    [RelayCommand]
    private void LoadCatalog()
    {
        Catalog.Clear();

        foreach (Book book in _bookBackend.Load())
            Catalog.Add(book);

        StatusMessage = $"Loaded {Catalog.Count} book(s).";
    }

    [RelayCommand]
    private void AddBook()
    {
        if (!IsBookInputValid())
            return;

        if (Catalog.Any(book => book.Isbn.Equals(BookIsbn, StringComparison.OrdinalIgnoreCase)))
        {
            StatusMessage = "A book with this ISBN already exists.";
            return;
        }

        Book newBook = new()
        {
            Id = Guid.NewGuid().ToString("N"),
            Title = BookTitle.Trim(),
            Author = BookAuthor.Trim(),
            Isbn = BookIsbn.Trim(),
            CopiesAvailable = BookCopiesAvailable
        };

        Catalog.Add(newBook);
        PersistCatalog();

        StatusMessage = "Book added successfully.";
        ClearBookForm();
    }

    [RelayCommand]
    private void EditBook()
    {
        if (SelectedBook is null)
        {
            StatusMessage = "Select a book to edit.";
            return;
        }

        if (!IsBookInputValid())
            return;

        bool duplicateIsbnExists = Catalog.Any(book =>
            !book.Id.Equals(SelectedBook.Id, StringComparison.Ordinal) &&
            book.Isbn.Equals(BookIsbn, StringComparison.OrdinalIgnoreCase));

        if (duplicateIsbnExists)
        {
            StatusMessage = "Another book already uses this ISBN.";
            return;
        }

        SelectedBook.Title = BookTitle.Trim();
        SelectedBook.Author = BookAuthor.Trim();
        SelectedBook.Isbn = BookIsbn.Trim();
        SelectedBook.CopiesAvailable = BookCopiesAvailable;

        PersistCatalog();
        LoadCatalog();
        StatusMessage = "Book updated successfully.";
    }

    [RelayCommand]
    private void DeleteBook()
    {
        if (SelectedBook is null)
        {
            StatusMessage = "Select a book to delete.";
            return;
        }

        string deletedTitle = SelectedBook.Title;
        Catalog.Remove(SelectedBook);
        PersistCatalog();

        SelectedBook = null;
        ClearBookForm();
        StatusMessage = $"Deleted '{deletedTitle}'.";
    }

    [RelayCommand]
    private void LoadActiveLoans()
    {
        ActiveLoans.Clear();

        List<LoanRecord> activeLoanRecords = _loanBackend
            .Load()
            .Where(record => !record.IsReturned)
            .OrderBy(record => record.DueAtUtc)
            .ToList();

        foreach (LoanRecord loanRecord in activeLoanRecords)
            ActiveLoans.Add(loanRecord);

        StatusMessage = $"Loaded {ActiveLoans.Count} active loan(s).";
    }

    partial void OnSelectedBookChanged(Book? value)
    {
        if (value is null)
            return;

        BookTitle = value.Title;
        BookAuthor = value.Author;
        BookIsbn = value.Isbn;
        BookCopiesAvailable = value.CopiesAvailable;
    }

    private bool IsBookInputValid()
    {
        if (string.IsNullOrWhiteSpace(BookTitle) ||
            string.IsNullOrWhiteSpace(BookAuthor) ||
            string.IsNullOrWhiteSpace(BookIsbn))
        {
            StatusMessage = "Title, author, and ISBN are required.";
            return false;
        }

        if (BookCopiesAvailable < 0)
        {
            StatusMessage = "Copies available cannot be negative.";
            return false;
        }

        return true;
    }

    private void PersistCatalog()
    {
        _bookBackend.SaveAll(Catalog.ToList());
    }

    private void ClearBookForm()
    {
        BookTitle = string.Empty;
        BookAuthor = string.Empty;
        BookIsbn = string.Empty;
        BookCopiesAvailable = 1;
    }

    [RelayCommand]
    private void Logout()
    {
        LogoutRequested?.Invoke();
    }
}