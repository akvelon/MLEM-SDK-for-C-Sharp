# MLEM API Client

## What is MLEM?

MLEM, developed by iterative.ai - is an open-source tool that allows users to simplify machine learning model packaging and deployment. With MLEM, users can run their machine learning models anywhere, by wrapping models as a Python package or Docker Image, or deploying them to Heroku (SageMaker, Kubernetes, and more platforms coming soon). 
MLEM is written in Python, and runs as a microservice with applications calling for a simple HTTP API. This may concern some users who did not develop their applications in Python, which is why we decided to take a closer look at using models on different platforms. 

## What is the .NET Client for MLEM?

This .NET HTTP client is designed to help software developers use a machine learning approach to create intelligent applications, based on .NET, without needing a deep knowledge of MLEM.This client is a library that allow users to connect with MLEM API simply and easily, as well as integrate the MLEM model functionality to their .NET project. It also supports request body validation by schema received from the MLEM server, and also offers a standard logging interface to use.

The client provides several methods for using MLEM technologies with given rules. The main of them are `PredictAsync` and `CallAsync`.
- `PredictAsync` makes a request for the "predict" method
- `CallAsync` makes a request for any methods (including "predict")

This is the core functionality of MLEM client for .NET apps. Having a stable application with minimum functionality is a good way to support it now and make for easier improvements in the future.

⚠️ There is a [Java client](https://github.com/akvelon/MLEM-SDK-for-Java) that does the same for Java projects.

## Getting Started
Before using the client make sure that you have a deployed mlem model (local or remote). Read [MLEM docs](https://mlem.ai/doc/get-started) to know how to deploy a model. A list of sample models you can find below. Also, clone the repository and build the `MlemApi` project.

After you have a link to a deployed model, prepare your application that will use MLEM capabilities. Refer it to the built `MlemApi.dll`.

## Example of client usage

There is `Example` project in this perository as an usage example, where you can see several cases (see `Utilities/TestCases.cs`).

Next, a sample case with the `Iris` model is described.

**Define a request object class**

Create a C# class according to the deployed model.
```cs
    using Newtonsoft.Json;

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

**Create the client**

The simplest overload of the client requests a link to deployed model.
```cs
    MlemApiClient mlemClient = new("https://example-mlem-get-started-app.herokuapp.com");
```

**Define the input data**

You can use an object instance for a request or a list of the objects

```cs
    List<Iris> inputData = new
    {
        new Iris
        {
            SepalLength = 5.1,
            SepalWidth = 3.5,
            PetalLength = 1.4,
            PetalWidth = 0.2
        },
        new Iris
        {
            SepalLength = 7.6,
            SepalWidth = 3.0,
            PetalLength = 6.6,
        }
    };
```

**Send a request**

`PredictAsync`
```cs
    List<long> result = await mlemClient.PredictAsync<List<long>, Iris>(inputData);
```

or  `CallAsync`

```cs
    List<List<double>> result = await mlemClient.CallAsync<List<List<double>>, Iris>(
                "predict_proba",
                inputData);
```

## Validation

Mlem client provides validatiion functionality for request objects and response based on api schema of the deployed mlem model. Since the mlem server does not provide a verbosed error in case of data format error - this feature can be useful to catch data format issues easier.
While request object validation allows you to double-check that input data was provided properly to the client, the response validation helps to check that a mlem model returns a response according to the schema.
For the request validation it is required to pass a map from model column names to request data class property names (`modelColumnToPropNamesMap`), so the mlem client will be able to associate relevant data fields from a request object and schema. 

You can turn on/off this feature using `ArgumentTypesValidationIsOn` and `ResponseValidationIsOn` properties of the mlem client for request and response objects validation respectively (turned off by default). 

See more examples of validation cases/input data prepared in the `Example` project.

## Classes generation

There is an upcoming feature for classes generation - for request and response data.
You can use `GenerateClasses` of `ModelClassesGenerator` classes to generate request and response data classes - to use it further in your add as data contracts.

See the `Example` project for the example of usage (`TestCases.ClassGeneration` test case).

## Sample ML models

There are the following sample models, that can be used for deployment and requests testing.
- Digits
- Iris
- Wine

They are in `ModelRepository` project in `Additional models` folder.

They are built using `LearnModelScript.py` scripts for each model.

## Conclusion

MLEM makes the process of packaging and deployment of machine learning models much easier. .NET client developed by Akvelon make it possible to integrate MLEM models to non-Python projects.
Use the client with your existing or new applications: 
Web (ASP.Net), Mobile (Xamarin), Desktop (WPF, WinForms). 
Forget about the need to create spaghetti code and get access to the advantages of MLEM’s features in your apps with the http clients developed by Akvelon's engineers!
