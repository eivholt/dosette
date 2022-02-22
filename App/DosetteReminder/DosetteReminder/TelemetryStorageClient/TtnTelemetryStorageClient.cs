using DosetteReminder.Extensions;
using DosetteReminder.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DosetteReminder.TelemetryStorageClient
{
    public partial class TtnTelemetryStorageClient : ITelemetryStorageClient
    {
        private string m_applicationId = "dosette"; // TTS application_id
        private readonly IHttpClientFactory m_httpClientFactory;

        public DateTime LastPollDateTime { get; private set; }
        public DateTime LastResponseCompletedDateTime { get; private set; }

        public TtnTelemetryStorageClient(IHttpClientFactory httpClientFactory)
        {
            m_httpClientFactory = httpClientFactory;
        }

        public async Task<List<TelemetryStorageMessage>> GetTelemetry()
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

            var orderedTelemetryData = telemetryData.OrderByDescending(x => DateTime.Parse(x.Result.ReceivedAt)).ToList();

            return orderedTelemetryData;
        }

        private static void AddUniqueMessages(List<TelemetryStorageMessage> telemetryDataList, TelemetryStorageMessage telemetryStorageMessage)
        {
            if(!telemetryDataList.Exists(x => x.Result.UplinkMessage.FCnt == telemetryStorageMessage.Result.UplinkMessage.FCnt))
            {
                telemetryDataList.Add(telemetryStorageMessage);
            }
        }
    }
}
