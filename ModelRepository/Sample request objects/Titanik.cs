using Newtonsoft.Json;

namespace ModelRepository.SampleRequestObjects
{
    public class Titanik
    {
        [JsonProperty("")]
        public int Id { get; set; }

        [JsonProperty("Pclass")]
        public int PClass { get; set; }

        [JsonProperty("Sex")]
        public int Sex { get; set; }

        [JsonProperty("Age")]
        public double Age { get; set; }

        [JsonProperty("SibSp")]
        public int Sibsp { get; set; }

        [JsonProperty("Parch")]
        public int Parch { get; set; }

        [JsonProperty("Fare")]
        public double Fare { get; set; }

        [JsonProperty("Embarked")]
        public int Embarked { get; set; }
    }
}
