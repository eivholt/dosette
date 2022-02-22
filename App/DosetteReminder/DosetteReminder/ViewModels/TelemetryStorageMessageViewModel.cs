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

        public bool Lid1Opened { get => m_message.Result.UplinkMessage.DecodedPayload.Lids.Lid1; }

        public bool Lid2Opened { get => m_message.Result.UplinkMessage.DecodedPayload.Lids.Lid2; }

        public bool Lid3Opened { get => m_message.Result.UplinkMessage.DecodedPayload.Lids.Lid3; }

        public bool Lid4Opened { get => m_message.Result.UplinkMessage.DecodedPayload.Lids.Lid4; }

        [Obsolete("Use DecodedPayload instead.")]
        private bool GetLidOpened(int lidNumber, string frmPayload)
        {
            var lidsByte = Convert.FromBase64String(frmPayload)[0];
            return (lidsByte & (1 << lidNumber-1)) != 0;
        }
    }
}
