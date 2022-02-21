using DosetteReminder.Models;
using DosetteReminder.TelemetryStorageClient;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DosetteReminder.ViewModels
{
    public class ReminderMainViewModel : BaseViewModel
    {
        private readonly ITelemetryStorageClient m_telemetryStorageClient;
        public ObservableCollection<TelemetryStorageMessage> TelemetryMessages { get; set; }

        public ICommand LoadTelemetryMessagesCommand => new AsyncCommand(ExecuteLoadTelemetryMessagesCommand);

        public ReminderMainViewModel(ITelemetryStorageClient telemetryStorageClient)
        {
            m_telemetryStorageClient = telemetryStorageClient;
            TelemetryMessages = new ObservableCollection<TelemetryStorageMessage>();
        }

        private async Task ExecuteLoadTelemetryMessagesCommand()
        {
            var telemetry = await m_telemetryStorageClient.GetTelemetry();

            foreach(var message in telemetry)
            {
                TelemetryMessages.Add(message);
            }
        }
    }
}
