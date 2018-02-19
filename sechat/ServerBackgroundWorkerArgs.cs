using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sechat
{
    /// <summary>
    /// Argument für den im ChatServer verwendeten
    /// BackgroundWorker
    /// </summary>
    /// <see cref="ChatServer"/>
    public class ServerBackgroundWorkerArgs
    {
        /// <summary>
        /// Zu verwendende Portnummer
        /// </summary>
        public int PortNumber = 0;
    }
}
