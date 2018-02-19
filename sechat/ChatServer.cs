using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;

namespace sechat
{
    /// <summary>
    /// ChatServer zum Empfangen von Nachrichten
    /// über einen TCP-Socket
    /// </summary>
    public class ChatServer
    {
        /// <summary>
        /// Deklaration des Delegates für den EventHandler, der
        /// den Nachrichtenempfang weitervermittelt
        /// </summary>
        /// <param name="sender">ChatServer, der die Nachricht empfangen hat</param>
        /// <param name="e">EventArgs, welche die empfangene Nachricht enthalten</param>
        public delegate void MessageReceivedEventHandler(ChatServer sender, MessageReceivedEventArgs e);

        /// <summary>
        /// Event, welches beim Empfang einer Nachricht ausgelöst wird
        /// </summary>
        public event MessageReceivedEventHandler MessageReceived;
        
        /// <summary>
        /// BackgroundWorker-Objekt, welches den Empfangsprozess in einen
        /// separaten Thread auslagert
        /// </summary>
        protected BackgroundWorker serverBackgroundWorker = null;

        /// <summary>
        /// Zur Verschlüsselung verwendeter Schlüssel
        /// </summary>
        private string key;

        ChatConnection connection = null;

        protected ChatServer() 
        {
            // BackgroundWorker erstellen (Progress aktivieren)
            serverBackgroundWorker = new BackgroundWorker { WorkerReportsProgress = true };

            // Benötigte Eventhandler registrieren
            serverBackgroundWorker.DoWork += new DoWorkEventHandler(serverBackgroundWorker_DoWork);
            serverBackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(serverBackgroundWorker_ProgressChanged);

        }

        /// <summary>
        /// Konstruktor mit Initialisierung des BackgroundWorker
        /// </summary>
        public ChatServer(string key)
            : this()
        {
            // Schlüssel speichern
            this.key = key;
        }

        /// <summary>
        /// Von außen zugängliche Methode zum Start
        /// des Empfangsvorgangs
        /// </summary>
        /// <param name="connection">Verbindungsdaten (z.Zt. wird nur die Portnummer verwendet)</param>
        public void BeginListen(ChatConnection connection)
        {
            this.connection = connection;

            // BackgroundWorker starten
            try
            {
                serverBackgroundWorker.RunWorkerAsync(new ServerBackgroundWorkerArgs
                {
                    PortNumber = connection.PortNumber
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Eventhandler für den Betrieb des BackgroundWorker
        /// </summary>
        /// <param name="sender">BackgroundWorker</param>
        /// <param name="e">ServerBackgroundWorkerArgs, welche insb. die zu verwendende Portnummer enthalten</param>
        protected virtual void serverBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Argument in ServerBackgroundWorkerArgs casten
            ServerBackgroundWorkerArgs args = e.Argument as ServerBackgroundWorkerArgs;

            // TcpListener zurücksetzen
            TcpListener tcpListener = null;

            try
            {
                // TcpListener erzeugen und starten
                tcpListener = new TcpListener(IPAddress.Loopback, args.PortNumber);
                tcpListener.Start();

                // Byte-Array als Buffer erzeugen
                Byte[] buffer = new Byte[2048];
                string stringBuffer = null;

                // Loop zum Warten auf Datenempfang
                while (true)
                {
                    // Warten auf Verbindung
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                        
                    // Buffer zurücksetzen
                    stringBuffer = null;
                    int numReceivedBytes = 0;

                    tcpClient.Client.RemoteEndPoint.ToString();

                    // Bereitgestellten Stream lesen
                    NetworkStream stream = tcpClient.GetStream();

                    while ((numReceivedBytes = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        // Empfangene Bytes in String konvertieren
                        stringBuffer = Encoding.UTF8.GetString(buffer, 0, numReceivedBytes);
                    }

                    // Nach Abschluss des Emfpangs empfange Nachricht über Progress verarbeiten
                    serverBackgroundWorker.ReportProgress(0, new ServerBackgroundWorkerProgress(stringBuffer));

                    // TcpClient schließen
                    tcpClient.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // TcpListener anhalten
                tcpListener.Stop();
            }
        }

        /// <summary>
        /// Eventhandler für Nachrichtenempfang im BackgroundWorker
        /// </summary>
        /// <param name="sender">BackgroundWorker</param>
        /// <param name="e">Enthält den UserState in Form eine ServerBackgroundWorkerProgress-Objekts,
        /// welches wiederum die empfangene Nachricht enthält</param>
        protected virtual void serverBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Wenn Eventhandler registriert wurden
            if (MessageReceived != null)
            {
                // UserState in ServerBackgroundWorkerProgress casten
                ServerBackgroundWorkerProgress progress = e.UserState as ServerBackgroundWorkerProgress;
                
                // Event auslösen
                MessageReceived(this, new MessageReceivedEventArgs 
                { 
                    Message = new ChatMessage(progress.Data, key) 
                });
            }
        }

    }
}
