using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TaskProTracker.Tests
{
        public class TaskApiTests : IClassFixture<WebApplicationFactory<Program>>
        {
            private readonly HttpClient _client;
            public TaskApiTests(WebApplicationFactory<Program> factory)
            {
                _client = factory.CreateClient();
            }

            [Fact]
            public async Task GetTasks_ReturnsOk()
            {
                var response = await _client.GetAsync("/tasks");
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            [Fact]
            public async Task PostTask_ReturnsCreated()
            {
                var newTask = new
                {
                    Title = "unit tesing API",
                    IsCompleted = true,
                    ProjectId = 4
                };

                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "your-jwt-token-here");

                var response = await _client.PostAsJsonAsync("/tasks", newTask);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            }
        }
    }

