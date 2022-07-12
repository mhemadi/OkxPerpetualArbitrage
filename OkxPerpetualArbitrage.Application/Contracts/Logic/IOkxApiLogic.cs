namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IOkxApiLogic
    {
        Task<decimal> GetAvailableUsdtBalance();
    }

}
