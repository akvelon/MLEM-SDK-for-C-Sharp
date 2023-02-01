namespace ModelRepository.SampleRequestObjects
{
    public class ValidationMaps
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

        public static Dictionary<string, string> lightGbmModelMap = new Dictionary<string, string>()
        {
            { "1", "One" },
            { "2", "Two" },
            { "3", "Three" },
            { "4", "Four" },
            { "5", "Five" },
            { "6", "Six" },
            { "7", "Seven" },
            { "8", "Eight" },
            { "9", "Nine" },
            { "10", "Ten" },
            { "11", "Eleven" },
            { "12", "Twelve" },
            { "13", "Thirteen" },
            { "14", "Fourteen" },
            { "15", "Fifteen" },
            { "16", "Sixteen" },
            { "17", "Seventeen" },
            { "18", "Eighteen" },
            { "19", "Nineteen" },
            { "20", "Twenty" },
            { "21", "TwentyOne" },
            { "22", "TwentyTwo" },
            { "23", "TwentyThree" },
            { "24", "TwentyFour" },
            { "25", "TwentyFive" },
            { "26", "TwentySix" },
            { "27", "TwentySeven" },
            { "28", "TwentyEight" },
        };
        }
}
