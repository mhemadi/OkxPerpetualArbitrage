namespace OkxPerpetualArbitrage.Application.Models.OkexApi
{
    public class OKEXResponse<T>
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

    }
}
