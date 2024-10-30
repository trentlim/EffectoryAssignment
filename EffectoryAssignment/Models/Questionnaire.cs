using System.Reflection.Metadata.Ecma335;
using EffectoryAssignment.Constants;

namespace EffectoryAssignment.Models
{
    public class Questionnaire
    {
        public int QuestionnaireId { get; set; }

        public IEnumerable<QuestionnaireItem>? QuestionnaireItems { get; set; }

        public IEnumerable<Question> GetAllQuestions()
        {
            if (QuestionnaireItems is null || QuestionnaireItems.Count() == 0)
            {
                return Enumerable.Empty<Question>();
            }

            return QuestionnaireItems
                .SelectMany(item => item.QuestionnaireItems?.OfType<Question>() ?? Enumerable.Empty<Question>());
        }

        public Question? GetQuestionById(int id)
        {
            var questions = GetAllQuestions();
            return questions.FirstOrDefault(item => item.QuestionId == id);
        }

        public Answer? GetAnswerById(int id)
        {
            var questions = GetAllQuestions();

            if (questions is null || questions.Count() == 0)
            {
                return null;
            }

            return questions
                .SelectMany(q => q.QuestionnaireItems?.OfType<Answer>() ?? Enumerable.Empty<Answer>())
                .FirstOrDefault(a => a.AnswerId == id);
        }

        public Response? AddResponseToAnswer(Response response, Answer answer)
        {
            if (answer.QuestionnaireItems is null)
            {
                answer.QuestionnaireItems = new List<QuestionnaireItem>();
            }

            answer.QuestionnaireItems = answer.QuestionnaireItems.Append(response).ToList();
            return response;
        }
    }
}
