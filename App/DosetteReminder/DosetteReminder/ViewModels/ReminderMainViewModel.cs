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
        public ObservableCollection<TelemetryStorageMessageViewModel> TelemetryMessages { get; set; }

        public TelemetryStorageMessageViewModel LastTelemetryMessage { get => TelemetryMessages.FirstOrDefault(); }

        public ICommand LoadTelemetryMessagesCommand => new AsyncCommand(ExecuteLoadTelemetryMessagesCommand);

        public ReminderMainViewModel(ITelemetryStorageClient telemetryStorageClient)
        {
            m_telemetryStorageClient = telemetryStorageClient;
            TelemetryMessages = new ObservableCollection<TelemetryStorageMessageViewModel>();
        }

        private async Task ExecuteLoadTelemetryMessagesCommand()
        {
            var telemetry = await m_telemetryStorageClient.GetTelemetry();

            foreach(var message in telemetry)
            {
                TelemetryMessages.Add(new TelemetryStorageMessageViewModel(message));
            }

            OnPropertyChanged(nameof(LastTelemetryMessage));
            OnPropertyChanged(nameof(TelemetryMessages));
        }
    }
}
