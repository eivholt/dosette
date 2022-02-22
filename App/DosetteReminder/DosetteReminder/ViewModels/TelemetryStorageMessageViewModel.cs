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

        public bool Lid2Opened { get => GetLidOpened(2, m_message.Result.UplinkMessage.FrmPayload); }

        public bool Lid3Opened { get => GetLidOpened(3, m_message.Result.UplinkMessage.FrmPayload); }

        public bool Lid4Opened { get => GetLidOpened(4, m_message.Result.UplinkMessage.FrmPayload); }

        private bool GetLidOpened(int lidNumber, string frmPayload)
        {
            var lidsByte = Convert.FromBase64String(frmPayload)[0];
            return (lidsByte & (1 << lidNumber-1)) != 0;
        }
    }
}
