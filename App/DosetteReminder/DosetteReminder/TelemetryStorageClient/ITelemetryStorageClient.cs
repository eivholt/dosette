using DosetteReminder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DosetteReminder.TelemetryStorageClient
{
    public interface ITelemetryStorageClient
    {
        Task<List<TelemetryStorageMessage>> GetTelemetry();

        DateTime LastPollDateTime { get; set; }
        DateTime LastResponseCompletedDateTime { get; set; }
    }
}
