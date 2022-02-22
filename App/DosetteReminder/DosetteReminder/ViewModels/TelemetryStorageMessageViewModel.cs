using DosetteReminder.Models;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DosetteReminder.ViewModels
{
    public class TelemetryStorageMessageViewModel : BaseViewModel
    {
        private TelemetryStorageMessage m_message;

        public TelemetryStorageMessageViewModel(TelemetryStorageMessage message)
        {
            m_message = message;
        }
        public DateTime ReceivedAt { get => DateTime.Parse(m_message.Result.ReceivedAt); }

        public bool Lid1Opened { get => GetLidOpened(1, m_message.Result.UplinkMessage.FrmPayload); }

        private bool GetLidOpened(int lidNumber, string frmPayload)
        {
            return Convert.FromBase64String(frmPayload)[0] == lidNumber;
        }
    }
}
