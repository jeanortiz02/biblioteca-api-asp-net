# BibliotecaAPI

A RESTful API for managing a library system, built with .NET.

## Features

- Manage books, authors, and users
- CRUD operations for library resources
- User authentication and authorization
- Search and filter functionality

## Getting Started

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download)
- SQL Server or SQLite

### Installation

1. Clone the repository:
    ```bash
    git clone
    cd BibliotecaAPI
    ```

2. Restore dependencies:
    ```bash
    dotnet restore
    ```

3. Update the connection string in `appsettings.json`.

4. Apply database migrations:
    ```bash
    dotnet ef migrations add <name of migration>
    dotnet ef database update
    ```

5. Run the API:
    ```bash
    dotnet watch run --launch-profile "https"
    ```

## API Endpoints

| Method | Endpoint         | Description           |
|--------|-----------------|-----------------------|
| GET    | /api/books      | List all books        |
| POST   | /api/books      | Add a new book        |
| PUT    | /api/books/{id} | Update a book         |
| DELETE | /api/books/{id} | Delete a book         |

## Contributing

Contributions are welcome! Please open issues or submit pull requests.

## License

This project is licensed under the MIT License.