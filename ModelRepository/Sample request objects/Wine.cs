using Newtonsoft.Json;

namespace ModelRepository.SampleRequestObjects
{
    internal class Wine
    {
        [JsonProperty("alcohol")]
        public double Alcohol { get; set; }

        [JsonProperty("malic_acid")]
        public double MalicAcid { get; set; }

        [JsonProperty("ash")]
        public double Ash { get; set; }

        [JsonProperty("alcalinity_of_ash")]
        public double AlcalinityOfAsh { get; set; }

        [JsonProperty("magnesium")]
        public double Magnesium { get; set; }

        [JsonProperty("total_phenols")]
        public double TotalPhenols { get; set; }

        [JsonProperty("flavanoids")]
        public double Flavanoids { get; set; }

        [JsonProperty("nonflavanoid_phenols")]
        public double NonflavanoidPhenols { get; set; }

        [JsonProperty("proanthocyanins")]
        public double Proanthocyanins { get; set; }

        [JsonProperty("color_intensity")]
        public double ColorIntensity { get; set; }

        [JsonProperty("hue")]
        public double Hue { get; set; }

        [JsonProperty("od280/od315_of_diluted_wines")]
        public double OD280_OD315_OfDilutedWines { get; set; }

        [JsonProperty("proline")]
        public double Proline { get; set; }
    }
}