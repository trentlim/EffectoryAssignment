using System.Security.Principal;
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

    return Results.Ok(questions);
});

app.Run();
