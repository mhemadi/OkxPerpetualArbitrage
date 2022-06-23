using OkxPerpetualArbitrage.BlazorUIExample.Models;
using OkxPerpetualArbitrage.BlazorUIExample.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;


namespace OkxPerpetualArbitrage.BlazorUIExample.Pages
{
    public partial class CurrentPositions
    {
        public List<CurrentPosition> Positions { get; set; } = new List<CurrentPosition>();
        public string Message { get; set; }
        public string StatusMessage { get; set; } = "";
        public bool IsLoading { get; set; } = false;
        public bool IsCancelRequestDisabled { get; set; } = false;
        public bool IsCloseRequestDisabled { get; set; } = false;
        public bool ShowStatusModal { get; set; } = false;
        public bool ShowClosePositionModal { get; set; } = false;
        public CloseRequestDto CloseRequest { get; set; } = new CloseRequestDto() { MinSpread = 0, Size = 0, Symbol = "" , IsInstant = false};
        public PositionDemandStatus Status { get; set; } = new PositionDemandStatus() { Filled = 0, IsCanceled = false, PositionDemandId = -1, Symbol = "", TotalSize = 0 };
        public OpenClosePositionData CloseData { get; set; } = new OpenClosePositionData() { MaxSize = 0, Spread = 0, Symbol = "" };
        public string CurrentSpreadSpanColor { get; set; } = "black";
        [Inject]
        public HttpClient HttpClient { get; set; }

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            await UpdatePostions();
            IsLoading = false;
        }

        private async Task Refresh()
        {
            IsLoading = true;
            await UpdatePostions(false);
            IsLoading = false;
        }
        private async Task RefreshWithError()
        {
            IsLoading = true;
            await UpdatePostions(true);
            IsLoading = false;

            if (Positions.Where(x => x.Error != "").Count() == 0)
            {
                StatusMessage = "NoErrors";
                StateHasChanged();
            }
           await  Task.Delay(2 * 1000);
            StatusMessage = "";
           // StateHasChanged();
        }
        private async Task UpdatePostions(bool withError = false)
        {
            try
            {
                string url = ApiUrl.Url+"/api/CurrentPosition";
                if(withError)
                    url = ApiUrl.Url+"/api/CurrentPositionCheckError";
                Positions = await HttpClient.GetFromJsonAsync<List<CurrentPosition>>(url);
            }
            catch (Exception ex)
            {
                Message = ex.Message + ex.StackTrace;
            }
        }


        private async void ShowModal(EventArgs e, bool inprogress, string symbol)
        {
            if (inprogress)
            {
                ResetStatus();
                IsCancelRequestDisabled = true;
                ShowStatusModal = true;
                await SetStatusData(symbol);
                IsCancelRequestDisabled = false;
                StateHasChanged();
            }
            else
            {
                ResetCloseData();
                IsCloseRequestDisabled = true;
                ShowClosePositionModal = true;
                await SetCloseData(symbol);
                IsCloseRequestDisabled = false;
                if (CloseData.Spread < 0)
                    CurrentSpreadSpanColor = "red";
                else
                    CurrentSpreadSpanColor = "green";
                StateHasChanged();
            }
        }

        private void CloseStatusModal()
        {
            ShowStatusModal = false;
        }
        private void CloseClosePositionModal()
        {
            ShowClosePositionModal = false;
        }

        private void ResetStatus()
        {
            Status.Filled = 0;
            Status.IsCanceled = false;
            Status.Symbol = "";
            Status.TotalSize = 0;
        }

        private void ResetCloseData()
        {
            CurrentSpreadSpanColor = "black";
            CloseData.MaxSize = 0;
            CloseData.Spread = 0;
            CloseData.Symbol = "";
        }

        private async Task SetStatusData(string symbol)
        {
            Status = await HttpClient.GetFromJsonAsync<PositionDemandStatus>(ApiUrl.Url+ $"/api/PositionDemand/{symbol}/Status");
        }
        private async Task SetCloseData(string symbol)
        {
            CloseData = await HttpClient.GetFromJsonAsync<OpenClosePositionData>(ApiUrl.Url+ $"/api/CurrentPosition/{symbol}/closeData");
        }

        private async Task ClosePosition(EventArgs e, string symbol, bool isInstant)
        {

            CloseRequest.Symbol = symbol;
            CloseRequest.IsInstant = isInstant;
            try
            {
                var r = await HttpClient.PostAsJsonAsync(ApiUrl.Url+ $"/api/CurrentPosition/{CloseRequest.Symbol}/close", CloseRequest);
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
            await UpdatePostions();
            ShowClosePositionModal = false;
         
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
            await UpdatePostions();
            ShowStatusModal = false;

            StateHasChanged();
            await Task.Delay(10 * 1000);
            StatusMessage = "";
        }

    }
}
