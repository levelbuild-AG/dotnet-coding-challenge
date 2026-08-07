# Customer API

A simple REST API for managing Customers and the Users that belong to them.
Built with .NET, ASP.NET Core, EF Core, and SQLite.

## What this does

- Create, view, update, and delete **Customers**
- Create, view, update, and delete **Users** — each User belongs to exactly one Customer
- Deleting a Customer automatically deletes all of its Users
- Supports filtering, sorting, and pagination on list endpoints (`$select`, `$filter`,
  `$top`, `$skip`, `$orderby`)
- Names and emails must be unique — trying to create a duplicate returns a clear error
- Data is stored in a local SQLite database file

## What you need before running this

- [.NET 8 SDK or newer](https://dotnet.microsoft.com/download) installed on your computer

## How to run it

Open a terminal in this folder and run:

```bash
cd CustomerApi
dotnet ef database update
dotnet run
```

The first command creates the database file (only needed the first time).
The second command starts the API.

Once it's running, you'll see a line like: http://localhost:5277
Open that address in your browser, or go to `http://localhost:5277/swagger`
for a page where you can try out every endpoint by clicking buttons.

## How to run the automated tests

```bash
cd CustomerApi.Tests
dotnet test
```

This runs a set of automated checks that confirm the core features work correctly —
creating records, blocking duplicates, returning proper errors, and cascade delete.

## A few decisions worth explaining

- **Why separate DTOs from database models?** So a Customer's response never
  accidentally includes its list of Users, and a User's response only shows the
  Customer's ID — not the Customer's full details — exactly as required.
- **Why does deleting a Customer also delete its Users?** This is enforced directly
  in the database itself, so it's guaranteed to happen consistently, not just
  something the application code has to remember to do.
- **Why SQLite?** It's a lightweight database that lives in a single file — no
  separate database server needs to be installed or running.
  


  ## Time spent

Approximately 7 hours total:
 2 hours: environment setup (.NET SDK, VS Code, project scaffolding) and researching
  concepts I hadn't used before (EF Core, OData, DTOs)
 2 hours: implementing the Customer and User models, DbContext, and both controllers
  with full CRUD support
 2 hours: writing and running automated tests
 1 hour: debugging, error handling, cleanup, and documentation
