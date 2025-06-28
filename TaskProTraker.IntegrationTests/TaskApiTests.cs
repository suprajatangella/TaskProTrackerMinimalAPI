using System.Net;
using System.Net.Http.Json;
using TaskProTracker.MinimalAPI.Dtos;
using TaskProTraker.IntegrationTests.Helpers;
using Xunit;

namespace TaskProTracker.IntegrationTests
{
    [Collection("Sequential")]
    public class TaskApiTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _httpClient;
        public TaskApiTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task GetTasks_ReturnsOk()
        {
            var response = await _httpClient.GetAsync("/tasks");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostTask_ReturnsCreated()
        {
            // Step 1: Login and get JWT token
            var loginData = new LoginUserDto
            {
                Email = "Admin@gmail.com",
                Password = "Admin@123"
            };

            var loginResponse = await _httpClient.PostAsJsonAsync("/login", loginData);
            loginResponse.EnsureSuccessStatusCode();

            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
            var token = loginResult?.Token;

            var newTask = new
            {
                Title = "integration tesing API",
                IsCompleted = true,
                ProjectId = 4
            };

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync("/tasks", newTask);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}

