using DosetteReminder.Extensions;
using DosetteReminder.Models;
using System.Web;

namespace DosetteReminder.TelemetryStorageClient
{
    public partial class TtnTelemetryStorageClient : TelemetryStorageClient
    {
        private readonly IHttpClientFactory m_httpClientFactory;

        public override DateTime LastPollDateTime { get; internal set; }
        public override DateTime LastResponseCompletedDateTime { get; internal set; }

        public TtnTelemetryStorageClient(IHttpClientFactory httpClientFactory)
        {
            m_httpClientFactory = httpClientFactory;
        }

        public override async Task<List<TelemetryStorageMessage>> GetTelemetry()
        {
            LastPollDateTime = DateTime.Now;
            var telemetryMessages = await GetTelemetryMessagesHttp();
            LastResponseCompletedDateTime = DateTime.Now;
            return telemetryMessages;
        }

        private async Task<List<TelemetryStorageMessage>> GetTelemetryMessagesHttp()
        {
            var httpClient = this.m_httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", m_authorizationKey);
            httpClient.DefaultRequestHeaders.Accept.Add(new("text/event-stream"));

            var uriBuilder = new UriBuilder($"https://eu1.cloud.thethings.network/api/v3/as/applications/{m_applicationId}/packages/storage/uplink_message");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["last"] = "48h";
            uriBuilder.Query = query.ToString();

            List<TelemetryStorageMessage> telemetryData = new List<TelemetryStorageMessage>();

            using (HttpResponseMessage response = await httpClient.GetAsync(uriBuilder.Uri, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    await foreach (TelemetryStorageMessage telemetryStorageMessage in stream!.ReadFromNdjsonAsync<TelemetryStorageMessage>())
                    {
                        AddUniqueMessages(telemetryData, telemetryStorageMessage);
                    }
                }
            }

            var orderedTelemetryData = OrderTelemetryMessagesByReceivedAt(telemetryData);

            return orderedTelemetryData;
        }
    }
}