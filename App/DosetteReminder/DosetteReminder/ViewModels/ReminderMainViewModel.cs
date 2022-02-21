using DosetteReminder.TelemetryStorageClient;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Windows.Input;

namespace DosetteReminder.ViewModels
{
    public class ReminderMainViewModel : BaseViewModel
    {
        private readonly ITelemetryStorageClient m_telemetryStorageClient;
        private string m_accessLog = "Access log";

        public ICommand LoadAccessLogCommand => new AsyncCommand(ExecuteLoadAccessLogCommand);

        public ReminderMainViewModel(ITelemetryStorageClient telemetryStorageClient)
        {
            m_telemetryStorageClient = telemetryStorageClient;
        }
        public string AccessLog
        {
            get => m_accessLog; 
            set => SetProperty(ref m_accessLog, value); 
        }

        private async Task ExecuteLoadAccessLogCommand()
        {
            var telemetry = await m_telemetryStorageClient.GetTelemetry();
            AccessLog = telemetry.Count.ToString();
        }
    }
}
