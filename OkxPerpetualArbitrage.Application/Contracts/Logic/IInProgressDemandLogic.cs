namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IInProgressDemandLogic
    {
        Task<decimal> GetTotalInProgressRequiredFunds();
    }
}
