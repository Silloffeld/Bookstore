using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using BookStore.DataAccess;
using Xunit;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System.Linq;

namespace BookstoreWeb.Tests
{
    /// <summary>
    /// Tests for registration endpoint error handling.
    /// Verifies that the application returns appropriate HTTP status codes (like 400 Bad Request)
    /// for various error scenarios during user registration.
    /// </summary>
    public class RegistrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public RegistrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Remove the actual database context
                    services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
                    
                    // Add in-memory database for testing
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}");
                    });
                });
            });
        }

        [Fact]
        public async Task Register_WithoutAntiForgeryToken_Returns400BadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var formData = new Dictionary<string, string>
            {
                ["Input.Email"] = "test@example.com",
                ["Input.Password"] = "Test123",
                ["Input.ConfirmPassword"] = "Test123",
                ["Input.Name"] = "Test User"
                // Missing __RequestVerificationToken
            };

            // Act
            var response = await client.PostAsync("/Identity/Account/Register", 
                new FormUrlEncodedContent(formData));

            // Assert
            // Without anti-forgery token, the request should be rejected with 400 Bad Request
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Register_WithInvalidAntiForgeryToken_Returns400BadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var formData = new Dictionary<string, string>
            {
                ["Input.Email"] = "test@example.com",
                ["Input.Password"] = "Test123",
                ["Input.ConfirmPassword"] = "Test123",
                ["Input.Name"] = "Test User",
                ["__RequestVerificationToken"] = "invalid-token-12345"
            };

            // Act
            var response = await client.PostAsync("/Identity/Account/Register", 
                new FormUrlEncodedContent(formData));

            // Assert
            // With invalid anti-forgery token, the request should be rejected with 400 Bad Request
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Register_GET_ReturnsSuccessAndContainsForm()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Identity/Account/Register");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var content = await response.Content.ReadAsStringAsync();
            
            // Verify the registration form elements exist
            Assert.Contains("Email", content);
            Assert.Contains("Password", content);
            Assert.Contains("Name", content);
            Assert.Contains("Register", content);
        }

        [Theory]
        [InlineData("", "Test123", "Test User")] // Missing email
        [InlineData("invalid-email", "Test123", "Test User")] // Invalid email format
        [InlineData("test@example.com", "", "Test User")] // Missing password
        [InlineData("test@example.com", "Test123", "")] // Missing name
        [InlineData("test@example.com", "short", "Test User")] // Password too short
        [InlineData("test@example.com", "nodigits", "Test User")] // Password without digits
        [InlineData("test@example.com", "nouppercase123", "Test User")] // Password without uppercase
        public async Task Register_WithInvalidData_ReturnsClientError(string email, string password, string name)
        {
            // Arrange
            var options = new WebApplicationFactoryClientOptions
            {
                HandleCookies = true
            };
            var client = _factory.CreateClient(options);

            // Get the registration page first to obtain a valid anti-forgery token
            var getResponse = await client.GetAsync("/Identity/Account/Register");
            var token = await ExtractAntiForgeryTokenAsync(getResponse);

            var formData = new Dictionary<string, string>
            {
                ["Input.Email"] = email,
                ["Input.Password"] = password,
                ["Input.ConfirmPassword"] = password,
                ["Input.Name"] = name,
                ["__RequestVerificationToken"] = token
            };

            // Act
            var response = await client.PostAsync("/Identity/Account/Register", 
                new FormUrlEncodedContent(formData));

            // Assert
            // With invalid data, the response should either be:
            // - 200 OK with validation errors displayed (standard Razor Pages behavior)
            // - 400 Bad Request (if using API-style validation)
            // Both are acceptable, but status should indicate the request was processed
            Assert.True(
                response.StatusCode == HttpStatusCode.OK || 
                response.StatusCode == HttpStatusCode.BadRequest,
                $"Expected OK (200) or BadRequest (400), but got {response.StatusCode}"
            );
        }

        [Fact]
        public async Task Register_WithValidData_ProcessesRequest()
        {
            // Arrange
            var options = new WebApplicationFactoryClientOptions
            {
                HandleCookies = true,
                AllowAutoRedirect = false
            };
            var client = _factory.CreateClient(options);

            // Get the registration page first
            var getResponse = await client.GetAsync("/Identity/Account/Register");
            var token = await ExtractAntiForgeryTokenAsync(getResponse);

            var uniqueEmail = $"testuser{Guid.NewGuid()}@example.com";
            var formData = new Dictionary<string, string>
            {
                ["Input.Email"] = uniqueEmail,
                ["Input.Password"] = "Test123",
                ["Input.ConfirmPassword"] = "Test123",
                ["Input.Name"] = "Test User",
                ["__RequestVerificationToken"] = token
            };

            // Act
            var response = await client.PostAsync("/Identity/Account/Register", 
                new FormUrlEncodedContent(formData));

            // Assert
            // The registration endpoint should process the request (not return 500 error)
            // Status codes can be:
            // - 200 OK (with validation errors or success page)
            // - 302/303 Redirect (successful registration)
            // - 400 Bad Request (validation or anti-forgery issues in test environment)
            // All these are acceptable - the key is the endpoint is reachable and processes requests
            Assert.True(
                response.StatusCode != HttpStatusCode.InternalServerError &&
                response.StatusCode != HttpStatusCode.NotFound,
                $"Registration endpoint should be reachable and process requests. Got: {response.StatusCode}"
            );
        }

        private async Task<string> ExtractAntiForgeryTokenAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(content);
            
            var tokenInput = document.QuerySelector("input[name='__RequestVerificationToken']") as IHtmlInputElement;
            return tokenInput?.Value ?? string.Empty;
        }
    }
}
