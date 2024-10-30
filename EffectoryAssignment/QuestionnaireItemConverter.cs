using EffectoryAssignment.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EffectoryAssignment
{
    public class QuestionnaireItemConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(QuestionnaireItem);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);

            if (!jo.TryGetValue("itemType", out JToken itemTypeToken))
            {
                throw new JsonSerializationException("No itemType property found in JSON.");
            }

            int itemType = itemTypeToken.Value<int>();
            QuestionnaireItem target;

            switch (itemType)
            {
                case 0:
                    target = new Subject();
                    break;
                case 1:
                    target = new Question();
                    break;
                case 2:
                    target = new Answer();
                    break;
                case 3:
                    target = new Response();
                    break;
                default:
                    throw new JsonSerializationException($"Unknown itemType: {itemType}");
            }

            serializer.Populate(jo.CreateReader(), target);
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Serialization is not implemented.");
        }
    }
}