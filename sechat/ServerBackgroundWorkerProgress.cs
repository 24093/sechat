using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sechat
{
    /// <summary>
    /// Progress-Objekt für den im ChatServer verwendeten
    /// BackgroundWorker (zum Verarbeiten einer empfangenen
    /// Nachricht)
    /// </summary>
    public class ServerBackgroundWorkerProgress
    {
        /// <summary>
        /// Empfangene Daten in Textform
        /// </summary>
        public string Data { get; private set; }

        /// <summary>
        /// Verwendete IP-Adresse/Port der aktuellen Nachricht
        /// </summary>
        public System.Net.EndPoint Endpoint;

        /// <summary>
        /// Einfacher Konstruktor
        /// </summary>
        /// <param name="data">Empfangene Daten in Textform</param>
        public ServerBackgroundWorkerProgress(string data)
        {
            this.Data = data;
        }
    }
}
