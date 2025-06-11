# 📚 BookStore API

A modern .NET 8 Web API for managing books with JWT authentication and SQLite database.

![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-green.svg)
![SQLite](https://img.shields.io/badge/Database-SQLite-lightblue.svg)
![JWT](https://img.shields.io/badge/Auth-JWT-orange.svg)

## 🚀 Features

- 📚 **CRUD Operations** - Create, Read, Update, Delete books
- 🔐 **JWT Authentication** - Secure token-based authentication
- 👤 **User Management** - Registration and login system
- 📖 **Swagger Documentation** - Interactive API testing interface
- 💾 **SQLite Database** - Lightweight database with Entity Framework Core
- 🧪 **Comprehensive Testing** - Full test coverage with unit and integration tests

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 8.0
- **Database**: SQLite + Entity Framework Core
- **Authentication**: JWT Bearer Tokens
- **Documentation**: Swagger/OpenAPI
- **Testing**: xUnit + Microsoft.AspNetCore.Mvc.Testing

## 🚀 Quick Setup

1. **Clone and navigate**
   ```bash
   git clone https://github.com/your-username/Book-Store-.NET.git
   cd Book-Store-.NET
   ```

2. **Run the application**
   ```bash
   dotnet restore
   dotnet ef database update
   dotnet run
   ```

3. **Access the API**
   - 🌐 **Swagger UI**: http://localhost:5000/swagger
   - 🔗 **API Base**: http://localhost:5000/api

## 📋 API Endpoints

### 🔐 Authentication
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `POST` | `/api/users/register` | Register new user | ❌ |
| `POST` | `/api/users/login` | User login | ❌ |

### 📚 Books Management  
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `GET` | `/api/books` | Get all books | ❌ |
| `GET` | `/api/books/{id}` | Get book by ID | ❌ |
| `POST` | `/api/books` | Create new book | ✅ |
| `PUT` | `/api/books/{id}` | Update existing book | ✅ |
| `DELETE` | `/api/books/{id}` | Delete book | ✅ |

## 🧪 Testing

Comprehensive test suite with **30 tests** covering all functionality:
- Unit Tests (19) - Services and controllers
- Integration Tests (11) - Full API workflows

**Run tests:**
```bash
dotnet test BookStore.sln
```

## 🧪 Testing the API

### Using Swagger UI (Recommended)
1. Navigate to http://localhost:5000/swagger
2. Click **"Try it out"** on any endpoint
3. For protected endpoints, click **"Authorize"** and enter your JWT token

### Using curl Commands

**📝 Register a new user:**
```bash
curl -X POST "http://localhost:5000/api/users/register" \
  -H "Content-Type: application/json" \
  -d '{"username": "testuser", "password": "password123"}'
```

**🔑 Login and get JWT token:**
```bash
curl -X POST "http://localhost:5000/api/users/login" \
  -H "Content-Type: application/json" \
  -d '{"username": "testuser", "password": "password123"}'
```

**📚 Create a book (requires JWT token):**
```bash
curl -X POST "http://localhost:5000/api/books" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{"title": "Clean Code", "author": "Robert Martin"}'
```

## 🗄️ Data Models

**📚 Book:**
```json
{
  "id": 1,
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "publishedDate": "2008-08-01T00:00:00Z"
}
```

**👤 User:**
```json
{
  "id": 1,
  "username": "john_doe",
  "password": "hashed_password"
}
```

## 🔧 Development

**Run in watch mode (auto-restart on changes):**
```bash
dotnet watch run
```

**Run tests in watch mode:**
```bash
dotnet watch test --project BooksApi.Tests/BooksApi.Tests.csproj
```

**Add new migration:**
```bash
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

---

**Built with .NET 8**

