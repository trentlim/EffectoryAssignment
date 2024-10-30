using Xunit;
using EffectoryAssignment.Models;
using System.Collections.Generic;
using System.Linq;

namespace EffectoryAssignment.Tests
{
    public class QuestionnaireTests
    {
        private Questionnaire CreateSampleQuestionnaire()
        {
            return new Questionnaire
            {
                QuestionnaireId = 1,
                QuestionnaireItems = new List<QuestionnaireItem>
                {
                    new Subject
                    {
                        SubjectId = 1,
                        OrderNumber = 1,
                        ItemType = 0,
                        QuestionnaireItems = new List<QuestionnaireItem>
                        {
                            new Question
                            {
                                QuestionId = 1,
                                SubjectId = 1,
                                OrderNumber = 1,
                                ItemType = 1,
                                AnswerCategoryType = 1,
                                QuestionnaireItems = new List<QuestionnaireItem>
                                {
                                    new Answer
                                    {
                                        AnswerId = 1,
                                        QuestionId = 1,
                                        OrderNumber = 1,
                                        ItemType = 2,
                                        AnswerType = 1
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        [Fact]
        public void GetAllQuestions_ReturnsEmptyList_WhenQuestionnaireItemsIsNull()
        {
            // Arrange
            var questionnaire = new Questionnaire
            {
                QuestionnaireId = 1,
                QuestionnaireItems = null
            };

            // Act
            var result = questionnaire.GetAllQuestions();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetAllQuestions_ReturnsAllQuestions_WhenQuestionsExist()
        {
            // Arrange
            var questionnaire = CreateSampleQuestionnaire();

            // Act
            var result = questionnaire.GetAllQuestions();

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result.First().QuestionId);
        }

        [Fact]
        public void GetQuestionById_ReturnsNull_WhenQuestionDoesNotExist()
        {
            // Arrange
            var questionnaire = CreateSampleQuestionnaire();

            // Act
            var result = questionnaire.GetQuestionById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetQuestionById_ReturnsQuestion_WhenQuestionExists()
        {
            // Arrange
            var questionnaire = CreateSampleQuestionnaire();

            // Act
            var result = questionnaire.GetQuestionById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.QuestionId);
        }

        [Fact]
        public void GetAnswerById_ReturnsNull_WhenAnswerDoesNotExist()
        {
            // Arrange
            var questionnaire = CreateSampleQuestionnaire();

            // Act
            var result = questionnaire.GetAnswerById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetAnswerById_ReturnsAnswer_WhenAnswerExists()
        {
            // Arrange
            var questionnaire = CreateSampleQuestionnaire();

            // Act
            var result = questionnaire.GetAnswerById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.AnswerId);
        }

        [Fact]
        public void AddResponseToAnswer_InitializesQuestionnaireItems_WhenNull()
        {
            // Arrange
            var questionnaire = CreateSampleQuestionnaire();
            var answer = new Answer
            {
                AnswerId = 2,
                QuestionId = 1,
                QuestionnaireItems = null
            };
            var response = new Response
            {
                ResponseId = 1,
                AnswerId = 2,
                UserId = 1,
                Department = "HR"
            };

            // Act
            var result = questionnaire.AddResponseToAnswer(response, answer);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(answer.QuestionnaireItems);
            Assert.Single(answer.QuestionnaireItems);
            Assert.Equal(response, answer.QuestionnaireItems.First());
        }

        [Fact]
        public void AddResponseToAnswer_AppendsResponse_WhenQuestionnaireItemsExists()
        {
            // Arrange
            var questionnaire = CreateSampleQuestionnaire();
            var answer = new Answer
            {
                AnswerId = 2,
                QuestionId = 1,
                QuestionnaireItems = new List<QuestionnaireItem>
                {
                    new Response { ResponseId = 1 }
                }
            };
            var response = new Response
            {
                ResponseId = 2,
                AnswerId = 2,
                UserId = 1,
                Department = "HR"
            };

            // Act
            var result = questionnaire.AddResponseToAnswer(response, answer);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, answer.QuestionnaireItems.Count());
            Assert.Contains(response, answer.QuestionnaireItems);
        }
    }
}