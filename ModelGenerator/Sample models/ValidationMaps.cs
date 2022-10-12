namespace ModelGenerator.Sample_models
{
    internal class ValidationMaps
    {
        public static Dictionary<string, string> irisColumnsMap = new Dictionary<string, string>()
        {
            { "sepal length (cm)", "SepalLength" },
            { "sepal width (cm)", "SepalWidth" },
            { "petal length (cm)", "PetalLength" },
            { "petal width (cm)", "PetalWidth"},
        };

        public static Dictionary<string, string> svmModelMap = new Dictionary<string, string>()
        {
            { "0", "Value" },
        };

        public static Dictionary<string, string> wineModelMap = new Dictionary<string, string>()
        {
            { "alcohol", "Alcohol" },
            { "malic_acid", "MalicAcid" },
            { "ash", "Ash" },
            { "alcalinity_of_ash", "AlcalinityOfAsh"},
            { "magnesium", "Magnesium"},
            { "total_phenols", "TotalPhenols"},
            { "flavanoids", "Flavanoids"},
            { "nonflavanoid_phenols", "NonflavanoidPhenols"},
            { "proanthocyanins", "Proanthocyanins"},
            { "color_intensity", "ColorIntensity"},
            { "hue", "Hue"},
            { "od280/od315_of_diluted_wines", "OD280_OD315_OfDilutedWines"},
            { "proline", "Proline"},
        };
    }
}
