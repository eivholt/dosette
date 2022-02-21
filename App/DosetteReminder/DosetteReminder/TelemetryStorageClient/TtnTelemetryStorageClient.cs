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
    public class TtnTelemetryStorageClient : ITelemetryStorageClient
    {
        private string m_authorizationKey = "NNSXS.5XOFCIXDM3BZWDB....."; // TTS API key
        private string m_applicationId = "dosette"; // TTS application_id
        private readonly IHttpClientFactory m_httpClientFactory;

        public DateTime LastPollDateTime { get; set; }
        public DateTime LastResponseCompletedDateTime { get; set; }

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
                    await foreach (TelemetryStorageMessage telemetryStorageMessage in response.Content!.ReadFromNdjsonAsync<TelemetryStorageMessage>())
                {
                    telemetryData.Add(telemetryStorageMessage);
                }
            }

            return telemetryData;
        }
    }
}
