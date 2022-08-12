namespace MlemApi
{
    public interface IMlemApiClient
    {
        Task<outcomeT> PredictAsync<incomeT, outcomeT>(List<incomeT> values);

        Task<outcomeT> CallAsync<incomeT, outcomeT>(string methodName, List<incomeT> values);
    }
}