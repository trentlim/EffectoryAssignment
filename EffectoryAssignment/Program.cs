using System.Reflection.Metadata.Ecma335;
using System.Security.Principal;
using EffectoryAssignment.Constants;
using EffectoryAssignment.Models;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();


var filePath = "questionnaire.json";
var questionnaire = LoadQuestionnaireData(filePath);

Questionnaire LoadQuestionnaireData(string filePath)
{
    try
    {
        var json = File.ReadAllText(filePath);
        var questionnaire = JsonConvert.DeserializeObject<Questionnaire>(json);

        return questionnaire ?? throw new InvalidOperationException("Failed to deserialize questionnaire");
    }
    catch (FileNotFoundException)
    {
        throw new FileNotFoundException($"Questionnaire file not found: {filePath}");
    }
    catch (Exception ex)
    {
        throw new Exception($"Error loading questionnaire from {filePath}: {ex.Message}", ex);
    }
}

app.MapGet("/questions", (int page = 1, int pageSize = 10) =>
{
    var questions = questionnaire.GetAllQuestions()
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();

    var json = JsonConvert.SerializeObject(questions);
    return Results.Ok(json);
});

app.MapGet("/questions/{id:int}", (int id) =>
{
    var question = questionnaire.GetQuestionById(id);
    var json = JsonConvert.SerializeObject(question);
    return question is not null ? Results.Ok(json) : Results.NotFound($"Question {id} not found");
});

// For open-ended questions
app.MapPost("questions/{questionId:int}/responses", (int questionId, int userId, string department, string text, string language) =>
{
    if (!Departments.ValidDepartments.Contains(department))
    {
        return Results.BadRequest($"Invalid department. Valid departments are: {string.Join(", ", Departments.ValidDepartments)}");
    }

    var question = questionnaire.GetQuestionById(questionId);
    if (question is null)
    {
        return Results.NotFound($"Question {questionId} not found");
    }

    Answer? answer = question.QuestionnaireItems?.OfType<Answer>().First();
    if (answer is null)
    {
        return Results.NotFound($"Answer not found");
    }

    // If it is an open-ended question and there is no text 
    if (string.IsNullOrWhiteSpace(text))
    {
        return Results.BadRequest("Text is required for open-ended questions");
    }

    var texts = new Dictionary<string, string>();
    texts.Add(language, text);

    var response = new Response
    {
        AnswerType = 2,
        UserId = userId,
        Department = department,
        Texts = texts,
    };

    var result = questionnaire.AddResponseToAnswer(response, answer);
    if (result is null)
    {
        return Results.Problem("An error occured");
    }
    else
    {
        return Results.Ok();
    }
});

// For multi-choice questions
app.MapPost("questions/{questionId:int}/answers/{answerId:int}/responses", (int questionId, int answerId, int userId, string department) =>
{
    if (!Departments.ValidDepartments.Contains(department))
    {
        return Results.BadRequest($"Invalid department. Valid departments are: {string.Join(", ", Departments.ValidDepartments)}");
    }

    var question = questionnaire.GetQuestionById(questionId);
    if (question is null)
    {
        return Results.NotFound($"Question {questionId} not found");
    }

    var answer = questionnaire.GetAnswerById(answerId);
    if (answer is null)
    {
        return Results.NotFound($"Answer {answerId} not found");
    }

    var response = new Response
    {
        AnswerId = answerId,
        AnswerType = 1,
        UserId = userId,
        Department = department,
    };

    var result = questionnaire.AddResponseToAnswer(response, answer);
    if (result is null)
    {
        return Results.Problem("An error occured");
    }
    else
    {
        return Results.Ok();
    }
});

app.Run();
public partial class Program { }
