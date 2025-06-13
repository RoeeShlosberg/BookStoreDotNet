# 📚 BookStore Application

🚀 A full-stack bookstore app built to demonstrate modern web development with a robust .NET 8 backend and a sleek Angular 17 frontend. Includes authentication, CRUD operations, unit tests, and responsive UI design. This project was created to expand my experience with the .NET ecosystem before moving to a React + Node.js stack.

---

## 🏗️ Project Structure

```
📁 BookStore/
├── 🖥️ server_dotNet/          # .NET 8 Web API backend
└── 🌐 client_angular/         # Angular 17 frontend
```

---

## 🖥️ .NET 8 REST API Backend

### 🛠️ Technologies Used
- .NET 8 + ASP.NET Core Web API
- Entity Framework Core + SQLite
- JWT Authentication
- Swagger for API docs
- xUnit for unit testing

### 🔧 Setup Instructions

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/BookStore.git
   cd BookStore/server_dotNet
   ```

2. **Update database connection**
   Edit `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Data Source=books.db"
   }
   ```

3. **Run database migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the backend**
   ```bash
   dotnet run
   ```
   - API runs at: http://localhost:5000
   - Swagger docs: http://localhost:5000/swagger

### 🔐 API Endpoints

**Auth**
- `POST /api/auth/register` – Register new user
- `POST /api/auth/login` – Login and get JWT

**Books**
- `GET /api/books` – Get all books
- `GET /api/books/{id}` – Get book by ID
- `POST /api/books` – Add a book (auth required)
- `PUT /api/books/{id}` – Edit a book (auth required)
- `DELETE /api/books/{id}` – Delete a book (auth required)

### ✅ Unit Testing (xUnit)

Covers:
- Auth services (login, JWT, hashing)
- Book service (CRUD, validation)
- Controllers (status codes, auth, errors)

Run tests:
```bash
cd server_dotNet
dotnet test
```

Code coverage:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

---

## 🌐 Angular 17 Frontend

### 🧰 Technologies Used
- Angular 17 + TypeScript
- Angular Material
- RxJS
- JWT client-side integration
- Responsive design

### 🔧 Setup Instructions

1. **Navigate to frontend**
   ```bash
   cd BookStore/client_angular
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Set API URL**
   In `src/environments/environment.ts`:
   ```typescript
   export const environment = {
     production: false,
     apiUrl: 'http://localhost:5000/api'
   };
   ```

4. **Run the app**
   ```bash
   ng serve
   ```
   App runs at: http://localhost:4200

### 🎯 Features
- Book browsing & searching
- JWT-based login/register
- Auth-guarded admin features
- Mobile-first responsive UI
- Angular Material components
- Lazy-loading & standalone components

### 🧪 Angular Testing

Includes:
- Unit tests for services & components
- Basic E2E flows
- UI rendering + validation

Run tests:
```bash
ng test
```

With coverage:
```bash
ng test --code-coverage
```

---

## ⚙️ Dev Workflow

1. Start backend (`dotnet watch run`)
2. Start frontend (`ng serve`)
3. Hot reload in both environments



## 👋 Final Notes

This project was built to get hands-on with .NET 8 + Angular 17 and simulate a real-world full-stack app with authentication, protected routes, testing, and database integration.

Feel free to fork, clone, or contribute!


