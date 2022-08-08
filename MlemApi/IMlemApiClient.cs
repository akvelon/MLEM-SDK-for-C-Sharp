namespace MlemApi
{
    public interface IMlemApiClient<incomeT, outcomeT>
    {
        Task<List<outcomeT>> PredictAsync(List<incomeT> values);
        Task<List<List<double>>> PredictProbabilityAsync(List<incomeT> values);
        Task<List<outcomeT>> SklearnPredictAsync(List<incomeT> values);
        Task<List<List<double>>> SklearnPredictProbabilityAsync(List<incomeT> values);
    }
}