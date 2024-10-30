using Newtonsoft.Json;

namespace EffectoryAssignment.Models
{
    [JsonConverter(typeof(QuestionnaireItemConverter))]
    public abstract class QuestionnaireItem
    {
        [JsonProperty(Order = 1)]
        public int OrderNumber { get; set; }

        [JsonProperty(Order = 2)]
        public Dictionary<string, string>? Texts { get; set; }

        [JsonProperty(Order = 2)]
        public int ItemType { get; set; }

        [JsonProperty(Order = 4)]
        public IEnumerable<QuestionnaireItem>? QuestionnaireItems { get; set; }
    }
    public class Subject : QuestionnaireItem
    {
        public int SubjectId { get; set; }

        public Subject()
        {
            ItemType = 0;
        }
    }
    public class Question : QuestionnaireItem
    {
        public int QuestionId { get; set; }

        public int SubjectId { get; set; }

        public int AnswerCategoryType { get; set; }

        public Question()
        {
            ItemType = 1;
        }

        public Answer? GetAnswerById(int id)
        {
            return QuestionnaireItems?
                .SelectMany(q => q.QuestionnaireItems?.OfType<Answer>() ?? Enumerable.Empty<Answer>())
                .FirstOrDefault(a => a.AnswerId == id);
        }
    }
    public class Answer : QuestionnaireItem
    {
        public int? AnswerId { get; set; }

        public int QuestionId { get; set; }

        public int AnswerType { get; set; }

        public Answer()
        {
            ItemType = 2;
        }
    }
    public class Response : QuestionnaireItem
    {
        public int ResponseId { get; set; }

        public int? AnswerId { get; set; }

        public int AnswerType { get; set; }

        public int UserId { get; set; }

        public string? Department { get; set; }

        public Response()
        {
            ItemType = 3;
        }
    }
}
