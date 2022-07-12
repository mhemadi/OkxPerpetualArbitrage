using OkxPerpetualArbitrage.BlazorUIExample.Models;
using OkxPerpetualArbitrage.BlazorUIExample.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace OkxPerpetualArbitrage.BlazorUIExample.Pages
{
    public partial class ResetPosition
    {
        public string Message { get; set; } = "";
        public List<string> Symbols { get; set; } = new List<string>();
        public string StatusMessage { get; set; } = "";
        public string SelectedValue { get; set; } = "";

        [Inject]
        public HttpClient HttpClient { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await FillSelect();
        }

        private async Task FillSelect()
        {
            try
            {
                Symbols = await HttpClient.GetFromJsonAsync<List<string>>(ApiUrl.Url+"/api/PotentialPosition/Symbol");
            }
            catch (Exception ex)
            {
                Message = ex.Message + ex.StackTrace;
            }
        }
        private async Task Reset(EventArgs e, string symbol)
        {

            try
            {
                var r = await HttpClient.PostAsJsonAsync(ApiUrl.Url+$"/api/CurrentPosition/{symbol}/reset", symbol);
                var resposnseStr = await r.Content.ReadAsStringAsync();
                ApiResponse response = System.Text.Json.JsonSerializer.Deserialize<ApiResponse>(resposnseStr);
                if (response.success)
                    StatusMessage = "Success. " + response.message;
                else
                    StatusMessage = "Failed. " + response.message;

            }
            catch (Exception ex)
            {
                StatusMessage = Environment.NewLine + ex.Message + ex.StackTrace;
            }
            StateHasChanged();
            await Task.Delay(10 * 1000);
            StatusMessage = "";

        }
    }
}
