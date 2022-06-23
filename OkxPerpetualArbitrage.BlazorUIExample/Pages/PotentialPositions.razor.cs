using OkxPerpetualArbitrage.BlazorUIExample.Models;
using OkxPerpetualArbitrage.BlazorUIExample.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace OkxPerpetualArbitrage.BlazorUIExample.Pages
{
    public partial class PotentialPositions
    {
        public List<PotentialPosition> PPs { get; set; }
        public bool ShowStatusModal { get; set; } = false;
        public bool ShowNewPositionModal { get; set; } = false;
        public string StatusMessage { get; set; } = "";
        public bool IsCancelRequestDisabled { get; set; } = false;
        public bool IsOpenRequestDisabled { get; set; } = false;
        public string CurrentSpreadSpanColor { get; set; } = "black";
        public string Message { get; set; } = "";
        public bool IsLoading { get; set; } = false;

        public OpenRequestDto OpenRequest { get; set; } = new OpenRequestDto() { MinSpread = 0, Size = 0, Symbol = "" };

        public PositionDemandStatus Status { get; set; } = new PositionDemandStatus() {Filled =0, IsCanceled = false, PositionDemandId =-1, Symbol = "", TotalSize =0 };
        public OpenClosePositionData OpenData { get; set; } = new OpenClosePositionData() { MaxSize =0, Spread = 0, Symbol ="" };
        [Inject]
        public HttpClient HttpClient { get; set; }


        protected override async Task OnInitializedAsync()
        {
           await  UpdatePPs();
        }

        private async Task UpdatePPs()
        {
            try
            {
                PPs = await HttpClient.GetFromJsonAsync<List<PotentialPosition>>(ApiUrl.Url+"/api/PotentialPosition");
            }
            catch (Exception ex)
            {
                Message = ex.Message + ex.StackTrace;
            }
        }
        private async Task Refresh()
        {
            IsLoading = true;
            await UpdatePPs();
            IsLoading = false;
        }

        private async Task SetStatusData(string symbol)
        {
            Status = await HttpClient.GetFromJsonAsync< PositionDemandStatus>(ApiUrl.Url+ $"/api/PositionDemand/{symbol}/Status");
        }
        private async Task SetOpenData(string symbol)
        {
            OpenData = await HttpClient.GetFromJsonAsync<OpenClosePositionData>(ApiUrl.Url+ $"/api/PotentialPosition/{symbol}/openData");
        }

        private async Task OpenPosition(EventArgs e, string symbol)
        {
         
            OpenRequest.Symbol = symbol;
            //  OpenRequestDto openRequestDto = new OpenRequestDto() {MinSpread = minSpred, Size = size, Symbol =symbol };
            try
            {
                var r = await HttpClient.PostAsJsonAsync(ApiUrl.Url+ $"/api/PotentialPosition/{OpenRequest.Symbol}/open", OpenRequest);
                var resposnseStr = await r.Content.ReadAsStringAsync();
                ApiResponse response = System.Text.Json.JsonSerializer.Deserialize<ApiResponse>(resposnseStr);
                if (response.success)
                    StatusMessage = "Success. " + response.message;
                else
                    StatusMessage = "Failed. " + response.message;

            }
            catch(Exception ex)
            {
                StatusMessage = Environment.NewLine + ex.Message + ex.StackTrace;
            }
            await UpdatePPs();
            ShowNewPositionModal = false;
   
            StateHasChanged();


            await Task.Delay(10 * 1000);
            StatusMessage = "";

        }

        private async Task CancelRequest(EventArgs e, int requestId)
        {
            try
            {
                var r = await HttpClient.PostAsync(ApiUrl.Url+ $"/api/PositionDemand/{requestId}/cancel", null);
                var resposnseStr = await r.Content.ReadAsStringAsync();
                ApiResponse response = System.Text.Json.JsonSerializer.Deserialize<ApiResponse>(resposnseStr);
                if (response.success)
                    StatusMessage = "Success. " + response.message;
                else
                    StatusMessage = "Failed. " + response.message;

            }
            catch (Exception ex)
            {
                StatusMessage += Environment.NewLine + ex.Message + ex.StackTrace;
            }
            await UpdatePPs();
            ShowStatusModal = false;
      
            StateHasChanged();
            await Task.Delay(10 * 1000);
            StatusMessage = "";
        }

    }
}
