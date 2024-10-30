namespace EffectoryAssignment.Models
{
    public class Questionnaire
    {
        public int QuestionnaireId { get; set; }

        public IEnumerable<QuestionnaireItem>? QuestionnaireItems { get; set; }

        public IEnumerable<Question> GetAllQuestions()
        {
            if (QuestionnaireItems == null || QuestionnaireItems.Count() == 0)
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
    }
}
