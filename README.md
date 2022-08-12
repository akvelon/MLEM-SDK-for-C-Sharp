# mlem-c-sharp

It is a library with MlemApiClient class which allows to interact with mlem API (https://mlem.ai/doc/get-started/serving). 
It allows to get prediction from ML model.
The library developed on C# with .NET 6.0 framework.

## MlemApiClient needs following params:
- httpClient - HTTPClient object, 
- configuraion - IMlemApiConfiguration object with addres of instance of mlem API in Url property,
- requestSerializer - object of class with IRequestValueSerializer interface which serialize input values to json string
- logger - object of class with ILogger interface for logging

## Methods:
- PredictAsync - call method with name "predict" of API
- CallAsync - call method with name methodName of API

## Methods params:
methodName - name of method to call,
- incomeT - type of income value, it is entity of ML model
- outcomeT - type of outcome value, it is result of ML model prediction 

## Features of work
- MlemApiClient vlidates methodName by information from interface.json data from API
- MlemApiClient build request by args name from interface.json data from API