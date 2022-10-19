﻿using ModelGenerator;
using Newtonsoft.Json;

namespace Example
{
    internal class IrisWIthMissingColumn: RequestModelType
    {
        [JsonProperty("sepal length (cm)")]
        public double SepalLength { get; set; }

        [JsonProperty("sepal width (cm)")]
        public double SepalWidth { get; set; }

        [JsonProperty("petal length (cm)")]
        public double PetalLength { get; set; }
    }
}