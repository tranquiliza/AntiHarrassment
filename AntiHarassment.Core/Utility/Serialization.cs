using Newtonsoft.Json;

namespace AntiHarassment.Core
{
    public static class Serialization
    {
        public static string Serialize<T>(T input)
        {
            return JsonConvert.SerializeObject(input);
        }

        public static T Deserialize<T>(string input)
        {
            return JsonConvert.DeserializeObject<T>(input);
        }
    }
}
