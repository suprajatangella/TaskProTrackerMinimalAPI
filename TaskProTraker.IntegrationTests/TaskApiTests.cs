using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
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

        public static IEnumerable<object[]> InvalidTasks => new List<object[]>
        {
            new object[] { new TaskItemDTO { Title = "no", ProjectId = 0, IsCompleted = false }, "ProjectId should be > 0"},
            new object[] { new TaskItemDTO { Title = "", ProjectId = 4, IsCompleted = false }, "Title is required." }
        };

        [Theory]
        [MemberData(nameof(InvalidTasks))]
        public async Task PostTaskWithValidationProblems(TaskItemDTO task, string errorMessage)
        {
            string? token = await GetJWTToken(_httpClient);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync("/tasks", task);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var errorResponse = JsonConvert.DeserializeObject<List<string>>(responseBody);

            Assert.NotNull(errorResponse);
            Assert.Collection(errorResponse, (error) => Assert.Equal(errorMessage, errorResponse.First()));
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
            string? token = await GetJWTToken(_httpClient);

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

        private async Task<string?> GetJWTToken(HttpClient _httpClient)
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
            return loginResult?.Token;
        }
    }
}

