using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace sechat
{
    /// <summary>
    /// ChatClient zum Senden von Nachrichten
    /// über einen TCP-Socket
    /// </summary>
    public class ChatClient
    {
        /// <summary>
        /// Internes TcpClient-Objekt für die
        /// Netzwerkkommunikation
        /// </summary>
        private TcpClient tcpClient = null;
        
        /// <summary>
        /// Verbindungsdaten für das Ziel der
        /// Übertragung
        /// </summary>
        private ChatConnection connection = null;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="connection">Verbindungseinstellungen für das Ziel der Übertragung</param>
        public ChatClient(ChatConnection connection)
        {
            // Verbindungsdaten speichern
            this.connection = connection;

            // TcpClient-Objekt erzeugen
            tcpClient = new TcpClient();
        }

        /// <summary>
        /// Nachricht senden
        /// </summary>
        /// <param name="message">Zu sendende Nachricht</param>
        /// <see cref="ChatMessage"/>
        public void Send(ChatMessage message)
        {
            // Verbindung herstellen
			try
			{
				tcpClient.Connect(connection.Address, connection.PortNumber);
				
				// Nachricht in Byte-Array konvertieren
				Byte[] byteMessage = Encoding.UTF8.GetBytes(message.GetMessage());

				// Byte-Array in den vom TcpClient bereitgestellten Stream
				// schreiben
				using (NetworkStream stream = tcpClient.GetStream())
				{
					stream.Write(byteMessage, 0, byteMessage.Count());
					stream.Close();
				}        				
            }
			catch (SocketException ex)
			{
				throw new Exception("Socket-Error " + ex.ErrorCode, ex);
			}
			catch (Exception)
			{
				throw;
			}
        }
    }
}
