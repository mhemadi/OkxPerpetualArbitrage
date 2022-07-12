namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface ITotalAvailableCloseSizeCalculatorLogic
    {
        Task<decimal> GetTotalAvailableSize(string symbol);
    }
}
