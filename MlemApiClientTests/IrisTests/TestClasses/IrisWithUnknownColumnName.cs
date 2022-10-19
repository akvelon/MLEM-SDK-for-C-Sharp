﻿using ModelGenerator;
using Newtonsoft.Json;

namespace MlemApiClientTests.IrisTests.TestClasses
{
    internal class IrisWithUnknownColumnName : RequestModelType
    {
        [JsonProperty("sepal length (cm)")]
        public double SepalLength { get; set; }

        [JsonProperty("sepal width (cm)")]
        public double SepalWidth { get; set; }

        [JsonProperty("petal length (cm)")]
        public double PetalLength { get; set; }

        [JsonProperty("some unknown column")]
        public double Unknown { get; set; }
    }
}