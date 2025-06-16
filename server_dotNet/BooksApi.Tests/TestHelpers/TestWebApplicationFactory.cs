using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace BooksApi.Tests.TestHelpers
{
    public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the app's DbContext registration
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<BooksDbContext>));

                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                // Remove any other DbContextOptions registrations that may exist
                var otherDbContextOptionsDescriptors = services.Where(
                    d => d.ServiceType.IsGenericType && 
                         d.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>)).ToList();

                foreach (var descriptor in otherDbContextOptionsDescriptors)
                {
                    services.Remove(descriptor);
                }

                // Add a clean SQLite in-memory database for testing
                services.AddDbContext<BooksDbContext>(options =>
                {
                    options.UseSqlite("DataSource=:memory:");
                });
                
                // Configure test authentication
                services.PostConfigureAll<JwtBearerOptions>(options =>
                {
                    options.BackchannelHttpHandler = new TestAuthHandler();
                });
                
                // Build service provider
                var sp = services.BuildServiceProvider();

                // Create a scope to get scoped services
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<BooksDbContext>();

                    // Ensure database is created and opened (important for SQLite in-memory)
                    db.Database.OpenConnection();
                    db.Database.EnsureCreated();

                    // Seed the database with test data
                    SeedDatabase(db);
                }
            });
        }

        private static void SeedDatabase(BooksDbContext context)
        {
            // Clear any existing data
            context.Books.RemoveRange(context.Books);
            context.Users.RemoveRange(context.Users);
            context.BookUsers.RemoveRange(context.BookUsers);
            context.SaveChanges();

            // Add a test user
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Password = "password123" // Use plain password for testing
            };
            context.Users.Add(user);
            
            // Add some test books
            var books = new List<Book>
            {
                new Book
                {
                    Id = 1,
                    Title = "Test Book 1",
                    Author = "Test Author 1",
                    UploadDate = DateTime.Now.AddDays(-5),
                    Rank = 5,
                    Categories = new List<string> { "Fantasy", "Mystery" }
                },
                new Book
                {
                    Id = 2,
                    Title = "Test Book 2",
                    Author = "Test Author 2",
                    UploadDate = DateTime.Now.AddDays(-3),
                    Rank = 4,
                    Categories = new List<string> { "Drama" }
                }
            };
            context.Books.AddRange(books);
            
            // Associate books with user
            var bookUsers = new List<BookUser>
            {
                new BookUser { BookId = 1, UserId = 1 },
                new BookUser { BookId = 2, UserId = 1 }
            };
            context.BookUsers.AddRange(bookUsers);
            
            context.SaveChanges();
        }
    }
    
    public class TestAuthHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwianRpIjoiNzVmOGU2N2UtNzdjYS00MTM0LWIxNjctMGQ5MTFiODdkMDdmIiwibmFtZWlkIjoiMSIsIm5hbWUiOiJ0ZXN0dXNlciIsIm5iZiI6MTY4MzEyMDQ1NywiZXhwIjoxNjgzMTI0MDU3LCJpYXQiOjE2ODMxMjA0NTcsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcxMTYiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjQyMDAifQ.Nw_XO3W4yiMSjQ_qhmqXoGlNpW9U--lTlw5jJZN-3BQ");
            return base.SendAsync(request, cancellationToken);
        }
    }
    
    public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "testuser")
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestScheme");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
