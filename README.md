# MLEM API Client

## Introduction

There is a MLEM (https://mlem.ai/) technology that helps you package and deploy machine learning models.
It saves ML models in a standard format that can be used in a variety of production scenarios such as real-time REST
serving or batch processing.

It is an unofficial C# library with MlemApiClient class which allows to interact with ML model by MLEM API (https://mlem.ai/doc/get-started/serving). 
It allows to get prediction from ML model.
The library developed on C# with .NET 6.0 framework.

## Client description

MlemApiClient provides API for using MLEM technologies in your code. There are two methods for making requests: 

**PredictAsync**
- sends /predict post request with serialized income values;
- can handle the exception;
- returns the response deserialized in the outcome object;
- works asynchronously;
- validates the income values;

**CallAsync**
- sends post request with given method name and serialized income values;
- can handle the exception;
- returns the response deserialized in the outcome object;
- works asynchronously;
- validates the income values;
- validates the method name by schema;

## Code Examples

**Request class**
```
    public class Iris
    {
        [JsonProperty("sepal length (cm)")]
        public double SepalLength { get; set; }

        [JsonProperty("sepal width (cm)")]
        public double SepalWidth { get; set; }

        [JsonProperty("petal length (cm)")]
        public double PetalLength { get; set; }

        [JsonProperty("petal width (cm)")]
        public double PetalWidth { get; set; }
    }
```
	
**Create client**
```
    var _mlemApiClient = new MlemApiClient("https://example-mlem-get-started-app.herokuapp.com");
```

**Do PredictAsync**
```
    await _mlemApiClient.PredictAsync<Iris, List<long>>(
        new List<Iris>
        {
            new Iris
            {
                SepalLength = -69639435.20838484,
                SepalWidth = 64887767.01179123,
                PetalLength = -76043679.89193763,
                PetalWidth = 20142568.61724788
            },
            new Iris
            {
                SepalLength = 6343387.454046518,
                SepalWidth = -30195626.60490367,
                PetalLength = 64042930.90411937,
                PetalWidth = -69196204.98948716
            }
        });
```

**Do CallAsync**
```
    await _mlemApiClient.CallAsync<Iris, List<List<double>>>("predict_proba"
        new List<Iris>
        {
            new Iris
            {
                SepalLength = -69639435.20838484,
                SepalWidth = 64887767.01179123,
                PetalLength = -76043679.89193763,
                PetalWidth = 20142568.61724788
            },
            new Iris
            {
                SepalLength = 6343387.454046518,
                SepalWidth = -30195626.60490367,
                PetalLength = 64042930.90411937,
                PetalWidth = -69196204.98948716
            }
        });
```
