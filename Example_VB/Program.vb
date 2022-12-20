Imports MlemApi
Imports Newtonsoft.Json

Public Class Iris
    Public Sub New(
                  ByVal SepalLength As Double,
                  ByVal SepalWidth As Double,
                  ByVal PetalLength As Double,
                  ByVal PetalWidth As Double
                  )
        Me.SepalLength = SepalLength
        Me.SepalWidth = SepalWidth
        Me.PetalLength = PetalLength
        Me.PetalWidth = PetalWidth
    End Sub
    <JsonProperty(PropertyName:="sepal length (cm)")>
    Public Property SepalLength As Double
    <JsonProperty(PropertyName:="sepal width (cm)")>
    Public Property SepalWidth As Double
    <JsonProperty(PropertyName:="petal length (cm)")>
    Public Property PetalLength As Double
    <JsonProperty(PropertyName:="petal width (cm)")>
    Public Property PetalWidth As Double
End Class
Module Program
    Sub Main(args As String())
        Dim mlemClient As New MlemApiClient("https://example-mlem-get-started-app.herokuapp.com")
        Dim inputValue As New Iris(3, 1.4, 7, 3)
        Dim res As List(Of Int32) = mlemClient.PredictAsync(Of List(Of Int32), Iris)(inputValue).GetAwaiter().GetResult()

        Console.WriteLine($"Result - {res.ElementAt(0)}")
    End Sub
End Module
