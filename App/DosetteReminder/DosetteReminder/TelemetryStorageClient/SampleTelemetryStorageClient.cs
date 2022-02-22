using DosetteReminder.Extensions;
using DosetteReminder.Models;

namespace DosetteReminder.TelemetryStorageClient
{
    public class SampleTelemetryStorageClient : TelemetryStorageClient
    {
        public override DateTime LastPollDateTime { get; internal set; }
        public override DateTime LastResponseCompletedDateTime { get; internal set; }

        public override async Task<List<TelemetryStorageMessage>> GetTelemetry()
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
                    AddUniqueMessages(telemetryData, telemetryStorageMessage);
                }
            }

            var orderedTelemetryData = OrderTelemetryMessagesByReceivedAtDescending(telemetryData);

            return orderedTelemetryData;
        }
    }
}