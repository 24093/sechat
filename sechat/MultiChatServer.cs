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
    /// Server für Chatrooms, d.h. Chats mit mehreren Beteiligten
    /// </summary>
    public class MultiChatServer : ChatServer
    {
        /// <summary>
        /// Liste mit Hosts, an die Nachrichten verteilt werden sollen
        /// </summary>
        private List<ChatConnection> clientList = new List<ChatConnection>();

        /// <summary>
        /// Fügt einen Host zur Liste hinzu, wenn er noch nicht
        /// vorhanden ist
        /// </summary>
        /// <param name="endPoint">IPEndPoint des Hosts</param>
        /// <param name="message">Nachricht (enthält Portnummer)</param>
        private void AddClient(EndPoint endPoint, string message)
        {
            ChatMessage msg = new ChatMessage(message, "");
            bool found = false;

            // Host in der Liste suchen
            foreach (ChatConnection connection in clientList)
            {
                if (connection.Address.Equals(((IPEndPoint)endPoint).Address) && connection.PortNumber == int.Parse(msg.PortNumber))
                {
                    found = true;
                    break;
                }
            }

            // Wenn nicht gefunden, hinzufügen
            if (!found)
            {
                ChatConnection conn = new ChatConnection 
                {
                    Address = ((IPEndPoint)endPoint).Address,
                    PortNumber = int.Parse(msg.PortNumber)
                };

                clientList.Add(conn);
            }
        }

        /// <summary>
        /// Entfernt einen Host aus der Liste
        /// </summary>
        /// <param name="endPoint">IPEndPoint des Hosts</param>
        private void RemoveClient(EndPoint endPoint)
        {
            ChatConnection index = null;

            // Host in der Liste suchen und speichern
            foreach (ChatConnection connection in clientList)
            {
                if (connection.Address == ((IPEndPoint)endPoint).Address)
                {
                    index = connection;
                    break;
                }
            }

            // Falls gefunden, entfernen
            if (index != null)
            {
                clientList.Remove(index);
            }
        }

        /// <summary>
        /// Eventhandler für den Betrieb des BackgroundWorker
        /// </summary>
        /// <param name="sender">BackgroundWorker</param>
        /// <param name="e">ServerBackgroundWorkerArgs, welche insb. die zu verwendende Portnummer enthalten</param>
        protected override void serverBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
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

                    // Bereitgestellten Stream lesen
                    NetworkStream stream = tcpClient.GetStream();

                    while ((numReceivedBytes = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        // Empfangene Bytes in String konvertieren
                        stringBuffer = Encoding.UTF8.GetString(buffer, 0, numReceivedBytes);
                    }

                    // Verbindung speichern
                    AddClient(tcpClient.Client.RemoteEndPoint, stringBuffer);

                    // Nach Abschluss des Emfpangs empfange Nachricht über Progress verarbeiten
                    ServerBackgroundWorkerProgress progress = new ServerBackgroundWorkerProgress(stringBuffer) 
                    {
                        Endpoint = tcpClient.Client.RemoteEndPoint
                    };

                    serverBackgroundWorker.ReportProgress(0, progress);

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
        protected override void serverBackgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            // UserState in ServerBackgroundWorkerProgress casten
            ServerBackgroundWorkerProgress progress = e.UserState as ServerBackgroundWorkerProgress;

            // Nachricht an jeden registrierten Host weiterleiten
            foreach (ChatConnection connection in clientList)
            {
                try
                {
                    // Nachricht senden, wird nicht entschlüsselt
                    ChatClient cc = new ChatClient(connection);
                    cc.Send(new ChatMessage(progress.Data, ""));
                }
                catch (Exception)
                {
                    // Falls nicht erreichbar, Host aus 
                    // der Liste löschen
                    RemoveClient(progress.Endpoint);
                }
            }
        }
    }
}
