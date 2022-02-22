using DosetteReminder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DosetteReminder.TelemetryStorageClient
{
    public abstract class TelemetryStorageClient : ITelemetryStorageClient
    {
        public abstract DateTime LastPollDateTime { get; internal set; }
        public abstract DateTime LastResponseCompletedDateTime { get; internal set; }

        public abstract Task<List<TelemetryStorageMessage>> GetTelemetry();

        internal static List<TelemetryStorageMessage> OrderTelemetryMessagesByReceivedAt(List<TelemetryStorageMessage> telemetryData)
        {
            return telemetryData.OrderBy(x => DateTime.Parse(x.Result.ReceivedAt)).ToList();
        }

        internal static void AddUniqueMessages(List<TelemetryStorageMessage> telemetryDataList, TelemetryStorageMessage telemetryStorageMessage)
        {
            if (!telemetryDataList.Exists(x => x.Result.UplinkMessage.FCnt == telemetryStorageMessage.Result.UplinkMessage.FCnt))
            {
                telemetryDataList.Add(telemetryStorageMessage);
            }
        }
    }
}
