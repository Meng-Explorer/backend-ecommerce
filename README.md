# Tech-Education (Backend E-Commerce API)

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat&logo=c-sharp&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-8.0-512BD4?style=flat)
![REST API](https://img.shields.io/badge/Architecture-Repository--Service-blue?style=flat)

A modular RESTful Web API built with **ASP.NET Core (.NET 8.0)** following the **Repository-Service pattern**. Designed to power the **Tech-Education / E-Commerce** platform with clean separation of concerns, Entity Framework Core migrations, custom middleware error handling, and Google OAuth 2.0 integration.

---

## ✨ Key Features

- **Layered Architecture (`Controller` - `Service` - `Repository`):** Decouples HTTP handling, business rules, and database persistence for high maintainability.
- **Data Transfer Objects (`DTOs`):** Strict separation between internal database entities (`Models`) and external API payloads.
- **Centralized Error Handling:** Custom domain exceptions (`Exceptions`) paired with request pipeline middlewares (`Middlewares`) for consistent API error responses.
- **Database Management:** Code-First database schema generation and version tracking via Entity Framework Core (`Data` & `Migrations`).
- **Authentication & Security:** Support for Google OAuth 2.0 authentication and environment-based configuration.

---

## 📂 Project Structure

```text
Tech-Education/
├── Controller/                  # REST API endpoints and route handlers
├── Data/                        # EF Core DbContext and database configurations
├── DTOs/                        # Request and Response Data Transfer Objects
├── Enum/                        # Application-wide enumerations and constants
├── Exceptions/                  # Custom exception definitions
├── Helpers/                     # Shared utility classes and extension methods
├── Middlewares/                 # Custom HTTP pipeline middlewares (e.g., global error handling)
├── Migrations/                  # EF Core Code-First database migrations
├── Models/                      # Domain entities and database table schemas
├── Properties/                  # Launch settings and assembly configuration
├── Repository/                  # Data access layer and repository interfaces/implementations
├── Service/                     # Core business logic and service interfaces/implementations
├── .gitignore                   # Git untracked file rules
├── appsettings.json             # Base application configuration
├── appsettings.Development.json # Development-specific configuration overrides
├── Program.cs                   # Application bootstrapper, DI container, and middleware pipeline
└── WepApi.http                  # HTTP test requests for quick endpoint verification
```

---

## 🛠️ Tech Stack

- **Framework:** ASP.NET Core Web API (.NET 8.0)
- **Language:** C#
- **ORM:** Entity Framework Core
- **Authentication:** Google OAuth 2.0 / JWT
- **API Testing:** Swagger / OpenAPI & `.http` client (`WepApi.http`)

---

## 🚀 Getting Started

### 1. Prerequisites

Ensure you have the following installed on your machine:
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A supported relational database (SQL Server / PostgreSQL / MySQL)
- An IDE such as **JetBrains Rider**, **Visual Studio 2022**, or **VS Code**

### 2. Clone the Repository

```bash
git clone [https://github.com/Meng-Explorer/backend-ecommerce.git](https://github.com/Meng-Explorer/backend-ecommerce.git)
cd backend-ecommerce
```

### 3. Configure Application Settings

Update `appsettings.Development.json` (or use **.NET User Secrets**) with your database connection string and Google OAuth credentials. **Do not commit real secrets to source control.**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TechEducationDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Authentication": {
    "Google": {
      "ClientId": "YOUR_GOOGLE_CLIENT_ID",
      "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

*(Optional)* To store secrets safely outside of Git using .NET User Secrets:
```bash
dotnet user-secrets init
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_GOOGLE_CLIENT_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_GOOGLE_CLIENT_SECRET"
```

### 4. Apply Database Migrations

Restore NuGet packages and apply Entity Framework Core migrations to create the database schema:

```bash
dotnet restore
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run
```

---

## 🧪 Testing the API

1. **Swagger UI:** Once the server is running in `Development` mode, open your browser and navigate to:
   ```text
   https://localhost:<port>/swagger
   ```
2. **HTTP Request File:** Open `WepApi.http` directly in JetBrains Rider, Visual Studio, or VS Code (with the REST Client extension) to send test requests to your endpoints.

---

## 👤 Author

**Meng-Explorer**
- GitHub: [@Meng-Explorer](https://github.com/Meng-Explorer)
