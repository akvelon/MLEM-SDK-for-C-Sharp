open MlemApi

type IrisRequestType(SepalLength: double, SepalWidth: double, PetalLength: double, PetalWidth: double) =
   member this.``sepal length (cm)`` = SepalLength
   member this.``sepal width (cm)`` = SepalWidth
   member this.``petal length (cm)`` = PetalLength
   member this.``petal width (cm)`` = PetalWidth
  
let input = new IrisRequestType(3, 1.4, 7, 3);

let mlemClient = new MlemApiClient("https://example-mlem-get-started-app.herokuapp.com");
let res = mlemClient.PredictAsync<'ResizeArray,'IrisRequestType>(input) |> Async.AwaitTask<ResizeArray<int>> |> Async.RunSynchronously
   
printfn "Result - %i" res[0]
