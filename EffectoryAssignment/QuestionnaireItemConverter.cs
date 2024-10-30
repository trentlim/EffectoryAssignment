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
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            JObject jo = new JObject();

            // Get all properties of the object
            var properties = value.GetType().GetProperties();

            foreach (var prop in properties)
            {
                // Skip QuestionnaireItems if it's null
                if (prop.Name == nameof(QuestionnaireItem.QuestionnaireItems) &&
                    prop.GetValue(value) == null)
                {
                    continue;
                }

                // Get the property value
                var propValue = prop.GetValue(value);
                if (propValue != null)
                {
                    // Handle special cases like Dictionary<string, string> for Texts
                    if (prop.Name == nameof(QuestionnaireItem.Texts) &&
                        propValue is Dictionary<string, string> texts)
                    {
                        jo.Add(prop.Name, JToken.FromObject(texts, serializer));
                    }
                    // Handle QuestionnaireItems collection
                    else if (prop.Name == nameof(QuestionnaireItem.QuestionnaireItems) &&
                            propValue is IEnumerable<QuestionnaireItem> items)
                    {
                        jo.Add(prop.Name, JToken.FromObject(items, serializer));
                    }
                    // Handle all other properties
                    else
                    {
                        jo.Add(prop.Name, JToken.FromObject(propValue, serializer));
                    }
                }
            }

            jo.WriteTo(writer);
        }
    }
}