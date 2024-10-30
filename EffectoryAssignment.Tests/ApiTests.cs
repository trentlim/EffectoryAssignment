using Microsoft.AspNetCore.Mvc.Testing;
using System.Text;
using System.Text.Json;
using Xunit;
using EffectoryAssignment.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace EffectoryAssignment.Tests
{
    public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetQuestions_ReturnsSuccessStatusCode()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/questions");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetQuestionById_ReturnsQuestion_WhenQuestionExists()
        {
            // Arrange
            var client = _factory.CreateClient();
            var questionId = 3851843;

            // Act
            var response = await client.GetAsync($"/questions/{questionId}");

            //Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var question = JsonConvert.DeserializeObject<Question>(content);
            Assert.NotNull(question);
            Assert.Equal(questionId, question.QuestionId);

        }

        [Fact]
        public async Task GetQuestionById_ReturnsSuccessStatusCode_WhenQuestionExists()
        {
            // Arrange
            var client = _factory.CreateClient();
            var questionId = 3851856;

            // Act
            var response = await client.GetAsync($"/questions/{questionId}");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetQuestionById_ReturnsNotFound_WhenQuestionDoesNotExist()
        {
            // Arrange
            var client = _factory.CreateClient();
            var nonExistentQuestionId = 99999;

            // Act
            var response = await client.GetAsync($"/questions/{nonExistentQuestionId}");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}