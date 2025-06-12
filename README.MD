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

### Authors

| Method | Endpoint            | Description                | Authentication |
| ------ | ------------------- | -------------------------- | -------------- |
| GET    | `/api/authors`      | List all authors           | Public         |
| GET    | `/api/authors/{id}` | Get an author by ID        | Public         |
| POST   | `/api/authors`      | Create a new author        | esadmin        |
| PUT    | `/api/authors/{id}` | Update an author           | esadmin        |
| PATCH  | `/api/authors/{id}` | Partially update an author | esadmin        |
| DELETE | `/api/authors/{id}` | Delete an author           | esadmin        |



### Authors Collection

| Method | Endpoint                        | Description                                                                | Authentication |
| ------ | ------------------------------- | -------------------------------------------------------------------------- | -------------- |
| GET    | `/api/authors-collection/{ids}` | Get a collection of authors by IDs (e.g., `/api/authors-collection/1,2,3`) | esadmin        |
| POST   | `/api/authors-collection`       | Create multiple authors in a single request                                | esadmin        |


### Books

| Method | Endpoint          | Description                             | Authentication |
| ------ | ----------------- | --------------------------------------- | -------------- |
| GET    | `/api/books`      | List all books                          | esadmin        |
| GET    | `/api/books/{id}` | Get a book by ID, including its authors | esadmin        |
| POST   | `/api/books`      | Create a new book                       | esadmin        |
| PUT    | `/api/books/{id}` | Update a book                           | esadmin        |
| DELETE | `/api/books/{id}` | Delete a book                           | esadmin        |



### Book Comments

| Method | Endpoint                            | Description                               | Authentication     |
| ------ | ----------------------------------- | ----------------------------------------- | ------------------ |
| GET    | `/api/books/{bookId}/comments`      | List comments for a book                  | Authenticated user |
| GET    | `/api/books/{bookId}/comments/{id}` | Get a specific comment of a book          | Authenticated user |
| POST   | `/api/books/{bookId}/comments`      | Create a new comment for a book           | Authenticated user |
| PATCH  | `/api/books/{bookId}/comments/{id}` | Partially update a comment *(owner only)* | Authenticated user |
| DELETE | `/api/books/{bookId}/comments/{id}` | Delete a comment *(owner only)*           | Authenticated user |


### Users / Authentication 

| Method | Endpoint                  | Description                                 | Authentication     |
| ------ | ------------------------- | ------------------------------------------- | ------------------ |
| POST   | `/api/users/register`     | Register a new user                         | Public             |
| POST   | `/api/users/login`        | Login and receive a JWT token               | Public             |
| PUT    | `/api/users`              | Update the authenticated user's information | Authenticated user |
| GET    | `/api/users/renew-token`  | Renew JWT token for authenticated user      | Authenticated user |
| POST   | `/api/users/make-admin`   | Grant admin role to a user                  | esadmin            |
| POST   | `/api/users/remove-admin` | Remove admin role from a user               | esadmin            |



#### Notes: 
* esadmin: Requires authentication and the user must have the esadmin claim.
* Authenticated user: Requires JWT authentication.
* Public: Accessible without authentication.
* Comment endpoints allow only the owner to update or delete their own comments.


## Contributing

Contributions are welcome! Please open issues or submit pull requests.

## License

This project is licensed under the MIT License.