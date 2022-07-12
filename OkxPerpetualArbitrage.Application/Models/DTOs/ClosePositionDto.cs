namespace OkxPerpetualArbitrage.Application.Models.DTOs
{
    public class ClosePositionDto : OpenClosePositionDto
    {
        public bool IsInstant { get; set; }
    }
}
