using System;
using System.Collections.Generic;
using System.Text;


namespace ChatBridge.Chat.Infrastructure.GlobalTimer
{
    public class GlobalMessageCooldown
    {
        private readonly object _lock = new();

        private DateTime _nextAllowedSendUtc = DateTime.MinValue;
        private bool _sendInProgress;

        public DateTime GetNextAllowedSendUtc()
        {
            lock (_lock)
            {
                return _nextAllowedSendUtc;
            }
        }

        public bool TryReserveSend(
            out DateTime nextAllowedSendUtc,
            out bool sendInProgress)
        {
            lock (_lock)
            {
                DateTime nowUtc = DateTime.UtcNow;

                sendInProgress = _sendInProgress;

                if (_sendInProgress || nowUtc < _nextAllowedSendUtc)
                {
                    nextAllowedSendUtc = _nextAllowedSendUtc;
                    return false;
                }

                _sendInProgress = true;
                nextAllowedSendUtc = _nextAllowedSendUtc;

                return true;
            }
        }

        public DateTime CompleteSend()
        {
            lock (_lock)
            {
                _nextAllowedSendUtc = DateTime.UtcNow.AddSeconds(5);
                _sendInProgress = false;

                return _nextAllowedSendUtc;
            }
        }

        public void CancelSend()
        {
            lock (_lock)
            {
                _sendInProgress = false;
            }
        }
    }
}
