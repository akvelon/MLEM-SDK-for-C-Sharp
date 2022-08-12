namespace MlemApi
{
    public interface IMlemApiClient<incomeT, outcomeT>
    {
        Task<List<outcomeT>> GetPredictAsync(string methodName, List<incomeT> values);
        Task<List<List<double>>> GetProbabilityAsync(string methodName, List<incomeT> values);
    }
}