using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sechat
{
    /// <summary>
    /// EventArgs für das MessageReceived-Event des ChatServers
    /// </summary>
    /// <see cref="ChatServer"/>
    public class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Empfangene Nachricht
        /// </summary>
        public ChatMessage Message = null;
    }
}
