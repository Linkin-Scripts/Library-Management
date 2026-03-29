# Library Management System

A desktop application for managing a library's book catalog and member loans, built with **Avalonia UI** and **.NET 10** following the **MVVM pattern**.

## Overview

The system supports two user roles:

| Role | Capabilities |
|------|-------------|
| **Member** | Browse catalog, search by title/author, borrow books (21-day loan), return books, view active loans |
| **Librarian** | Add/edit/delete books, view all active loans across all members, manage catalog |

Authentication uses SHA256 password hashing. All data is persisted locally as JSON files.

## Tech Stack

| Technology | Version | Purpose |
|-----------|---------|---------|
| .NET | 10.0 | Runtime |
| C# | 13 | Language |
| Avalonia UI | 11.3.12 | Cross-platform desktop UI framework |
| CommunityToolkit.Mvvm | 8.2.1 | MVVM source generation (`[ObservableProperty]`, `[RelayCommand]`) |
| System.Text.Json | (built-in) | JSON serialization |
| System.Security.Cryptography | (built-in) | SHA256 password hashing |

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Build & Run

```bash
# Clone the repository
git clone <repo-url>
cd Library-Management

# Run the application
dotnet run --project "Library Management App"

# Build for release
dotnet build -c Release
```

### Default Credentials

The application ships with a pre-seeded librarian account in `Data/users.json`. Check the JSON file for the default admin username — the password is stored as a SHA256 hash.

## Project Structure

```
Library Management App/
├── Models/                     # Domain entities
│   ├── IUser.cs                # User interface (GetUserInformation)
│   ├── User.cs                 # Concrete user (UserName, Password, Permission)
│   ├── Book.cs                 # Book (Id, Title, Author, Isbn, CopiesAvailable)
│   └── LoanRecord.cs           # Loan (BookId, MemberUserName, BorrowedAt, DueAt, IsReturned)
├── ViewModels/                 # Presentation logic (MVVM)
│   ├── ViewModelBase.cs        # Base class (ObservableObject)
│   ├── MainWindowViewModel.cs  # Navigation orchestrator
│   ├── LoginViewModel.cs       # Login + navigation to register
│   ├── RegisterViewModel.cs    # Registration + password hashing
│   ├── HomeViewModel.cs        # Placeholder home screen
│   └── LibrarianViewModel.cs   # Book CRUD + active loan list
├── Views/                      # Avalonia XAML UI
│   ├── MainWindow.axaml        # Shell window
│   ├── LoginView.axaml
│   ├── RegisterView.axaml
│   ├── HomeView.axaml
│   ├── LibrarianView.axaml
│   ├── MemberView.axaml
│   └── MemberViewModel.cs      # Member browse/search/borrow/return logic (misplaced here)
├── Data/                       # Persistence layer
│   ├── IFileBackend.cs         # Generic interface: Save / Load / SaveAll
│   ├── GenericFileBackend.cs   # JSON implementation for Book, LoanRecord
│   ├── FileBackend.cs          # Specialized JSON implementation for IUser
│   ├── books.json              # Book catalog (seeded with sample data)
│   ├── users.json              # User accounts
│   └── loans.json              # Loan history
├── Styles/                     # Shared XAML styles (Fluent theme)
├── Program.cs                  # Entry point (STAThread, Avalonia bootstrap)
├── App.axaml                   # App-level resources + ViewLocator registration
└── ViewLocator.cs              # Reflects ViewModel name to View type automatically
```

## Architecture

### MVVM Pattern

The application strictly follows MVVM:

- **Models** hold only data with no UI logic.
- **ViewModels** contain all business and presentation logic. They inherit from `ViewModelBase : ObservableObject` and use CommunityToolkit source generators:
  - `[ObservableProperty]` on private fields generates public two-way bindable properties.
  - `[RelayCommand]` on methods generates `ICommand` properties bound to buttons.
- **Views** are pure XAML with no code-behind logic. Bindings are compiled (`AvaloniaUseCompiledBindingsByDefault=true`).
- **ViewLocator** (`ViewLocator.cs`) automatically resolves `FooViewModel` to `FooView` by name convention via reflection, so no manual View registration is needed.

### Navigation

`MainWindowViewModel` is the single source of truth for navigation. It owns all ViewModel instances and exposes a `CurrentView` (`UserControl`) that the `MainWindow` binds to.

Navigation is event-driven:

```
LoginViewModel    --LoginSuccessful(permission)--> MainWindowViewModel
LoginViewModel    --RegisterClicked-------------> MainWindowViewModel
RegisterViewModel --LoginClicked---------------> MainWindowViewModel
LibrarianViewModel --LogoutRequested-----------> MainWindowViewModel
MemberViewModel   --LogoutRequested-------------> MainWindowViewModel
```

On successful login, `MainWindowViewModel` reads the `permission` string:
- `"librarian"` or `"admin"` → shows `LibrarianView`
- anything else → creates a new `MemberViewModel(username)` and shows `MemberView`

### Data Layer

All persistence is handled through a generic interface:

```csharp
public interface IFIleBackend<T>
{
    bool Save(T data);          // Append one item
    List<T> Load();             // Load all items
    bool SaveAll(List<T> all);  // Overwrite entire list
}
```

- **`GenericFileBackend<T>`** — serializes/deserializes any type `T` to a JSON array file. Used for `Book` and `LoanRecord`.
- **`FileBackend`** — implements `IFIleBackend<IUser>` with special handling to cast `IUser` ↔ `User` during serialization (since `System.Text.Json` cannot deserialize to an interface directly).

### Loan Logic

- **Borrow:** decrements `Book.CopiesAvailable`, creates a `LoanRecord` with `DueAtUtc = now + 21 days`, persists both files.
- **Return:** sets `LoanRecord.IsReturned = true`, increments `Book.CopiesAvailable`, persists both files.
- Librarian sees only `IsReturned == false` records, sorted by due date.

### Password Hashing

`RegisterViewModel.HashFunction(string password)` computes a SHA256 hex digest. Passwords are never stored in plaintext. `LoginViewModel` hashes the entered password before comparing it against the stored hash.

## Data File Formats

**`users.json`**
```json
[
  { "UserName": "admin", "Password": "<sha256-hex>", "Permission": "librarian" },
  { "UserName": "alice", "Password": "<sha256-hex>", "Permission": "user" }
]
```

**`books.json`**
```json
[
  {
    "Id": "abc123",
    "Title": "Clean Code",
    "Author": "Robert C. Martin",
    "Isbn": "9780132350884",
    "CopiesAvailable": 3
  }
]
```

**`loans.json`**
```json
[
  {
    "Id": "def456",
    "BookId": "abc123",
    "BookTitle": "Clean Code",
    "MemberUserName": "alice",
    "BorrowedAtUtc": "2026-03-01T10:00:00Z",
    "DueAtUtc": "2026-03-22T10:00:00Z",
    "IsReturned": false
  }
]
```

## Known Issues / Limitations

- `MemberViewModel.cs` is located inside `Views/` instead of `ViewModels/` — misplaced file.
- The persistence interface is named `IFIleBackend` (typo: double capital `F`).
- Data files are loaded from relative paths (`Data/books.json`), so the working directory must be the project folder when running.
- No automated tests exist in the project.
