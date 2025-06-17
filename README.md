# 📚 BookStore Application

🚀 A full-stack bookstore app built to demonstrate modern web development with a robust .NET 8 backend and a sleek Angular 17 frontend. Includes authentication, user-specific libraries, book categories, ratings, public shareable book lists, social media integration, comprehensive testing, and a full CI pipeline with GitHub Actions.

---

## 🏗️ Project Structure

```
📁 BookStore/
├── 🖥️ server_dotNet/          # .NET 8 Web API Backend
│   ├── Controllers/          # API endpoints (Books, Users, Categories, SharedLists)
│   ├── Services/             # Business logic
│   ├── Data/                 # EF Core DbContext and migrations
│   ├── Models/               # C# entity models (Book, User, Category, Rating, SharedBookList)
│   └── BooksApi.Tests/       # xUnit tests for services and controllers
├── 🌐 client_angular/         # Angular 17 Frontend
│   ├── src/app/
│   │   ├── components/       # Reusable UI components (e.g., book card, navbar)
│   │   ├── pages/            # Main views (e.g., home, login, book details, shared-list)
│   │   ├── services/         # Services for API communication and auth
│   │   └── guards/           # Route guards for authentication
└── ⚙️ .github/workflows/      # GitHub Actions CI workflow
```

---

## 🖥️ .NET 8 REST API Backend

The backend is a powerful and secure REST API built with ASP.NET Core 8, providing all the necessary functionality for the BookStore application.

### 🛠️ Key Technologies & Features

-   **.NET 8 & ASP.NET Core Web API**: For building high-performance, cross-platform APIs.
-   **Entity Framework Core & SQLite**: For data access and database management.
-   **JWT Authentication & Authorization**: Secure endpoints with JSON Web Tokens.
-   **Repository & Service Pattern**: Cleanly separates data access from business logic.
-   **Dependency Injection**: Loosely coupled and maintainable code.
-   **Swagger/OpenAPI**: Interactive API documentation.
-   **xUnit**: Comprehensive unit test coverage for services and controllers.
-   **GitHub Actions CI**: Automated build and test pipeline.

### 🔐 API Endpoints

**Users & Authentication (`/api/Users`)**

-   `POST /register`: Creates a new user account.
-   `POST /login`: Authenticates a user and returns a JWT.

**Books (`/api/Books`)**

-   `GET /`: Retrieves a list of all books, including their category and average rating.
-   `GET /{id}`: Fetches a single book by its ID.
-   `POST /`: Adds a new book to the collection (Authentication Required).
-   `PUT /{id}`: Updates an existing book (Authentication Required).
-   `DELETE /{id}`: Removes a book (Authentication Required).

**Categories (`/api/Categories`)**

-   `GET /`: Returns a list of all available book categories.

**Shared Lists (`/api/SharedLists`)**

-   `POST /`: Creates a shareable list of books and returns a unique ID.
-   `GET /{id}`: Retrieves a collection of books from a shared list using its unique ID.

### ✅ Testing & CI

A robust suite of unit tests built with xUnit ensures the reliability of the business logic. The tests cover:

-   **Service Layer**: Verifies all CRUD operations, user registration, and password hashing.
-   **Controller Layer**: Ensures correct HTTP status codes, route protection, and error handling.

**Continuous Integration (CI)** is handled by GitHub Actions. The workflow in `.github/workflows/dotnet.yml` automatically triggers on every push to the `main` branch to build the solution and run the full test suite, guaranteeing that new changes don't break existing functionality.

To run tests locally:

```bash
cd server_dotNet
dotnet test
```

---

## 🌐 Angular 17 Frontend

The frontend is a modern, responsive single-page application (SPA) built with Angular 17, offering a seamless and interactive user experience.

### 🧰 Key Technologies & Features

-   **Angular 17 & TypeScript**: A powerful framework for building dynamic and maintainable web applications.
-   **Angular Material**: A suite of high-quality UI components for a clean and professional look.
-   **RxJS**: For managing asynchronous operations and state.
-   **Component-Based Architecture**: Organized into pages, reusable components, services, and guards.
-   **Lazy Loading**: For optimized performance, loading feature modules on demand.
-   **JWT Client-Side Handling**: Securely stores and sends JWTs for authenticated API requests using an HTTP Interceptor.
-   **Route Guards**: Protects routes like the user's personal library, accessible only to logged-in users.
-   **Responsive Design**: Adapts beautifully to various screen sizes, from mobile to desktop.
-   **Public Sharing Feature**: Generate unique URLs for sharing curated book collections with anyone.
-   **Social Media Integration**: Built-in sharing capabilities for WhatsApp, Facebook, Twitter, and email.

### ✨ User Experience Flow

1.  **Register & Login**: Users can create an account and log in to access protected features.
2.  **Browse Books**: The main page displays a gallery of all available books.
3.  **View Details**: Clicking a book shows its full details, including its assigned **category** and user **ratings**.
4.  **Personal Library**: Authenticated users can add, edit, or remove books from their collection.
5.  **Share Book Lists**: Users can create public, shareable links to their filtered book collections.
6.  **Social Sharing**: Seamlessly share book lists via WhatsApp, Facebook, Twitter, or email with integrated social buttons.

### 🔧 Setup & Running

1.  **Navigate to the frontend directory**:
    ```bash
    cd client_angular
    ```
2.  **Install dependencies**:
    ```bash
    npm install
    ```
3.  **Run the development server**:
    ```bash
    ng serve
    ```
    The application will be available at `http://localhost:4200`.


