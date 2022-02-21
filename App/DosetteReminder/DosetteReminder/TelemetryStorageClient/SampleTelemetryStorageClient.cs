using DosetteReminder.Extensions;
using DosetteReminder.Models;
using System.Text.Json;

namespace DosetteReminder.TelemetryStorageClient
{
    public class SampleTelemetryStorageClient : ITelemetryStorageClient
    {
        public DateTime LastPollDateTime { get; set; }
        public DateTime LastResponseCompletedDateTime { get; set; }

        public async Task<List<TelemetryStorageMessage>> GetTelemetry()
        {
            LastPollDateTime = DateTime.Now;
            var telemetryMessages = await LoadMauiAsset("SampleData.ndjson");
            LastResponseCompletedDateTime = DateTime.Now;

            return telemetryMessages;
        }
        private async Task<List<TelemetryStorageMessage>> LoadMauiAsset(string fileName)
        {
            List<TelemetryStorageMessage> telemetryData = new List<TelemetryStorageMessage>();

            using (var stream = await FileSystem.OpenAppPackageFileAsync(fileName))
            {
                await foreach (TelemetryStorageMessage telemetryStorageMessage in stream!.ReadFromNdjsonAsync<TelemetryStorageMessage>())
                {
                    telemetryData.Add(telemetryStorageMessage);
                }
            }

            return telemetryData;
        }
    }
}
