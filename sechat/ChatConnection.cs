using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace sechat
{
    /// <summary>
    /// Verbindungsdaten für eine
    /// Netzwerkverbindung bestehend aus
    /// IP-Adresse und Portnummer
    /// </summary>
    public class ChatConnection
    {
        /// <summary>
        /// IP-Adresse (Standardwert ist 127.0.0.1)
        /// </summary>
        public IPAddress Address = IPAddress.Loopback;
        
        /// <summary>
        /// Portnummer (Standardwert ist 31338)
        /// </summary>
        public int PortNumber = 31338;

        /// <summary>
        /// Überschreiben der ToString()-Methode
        /// zeigt die Adresse und Port an
        /// </summary>
        /// <returns>String in der Form xxx.xxx.xxx.xxx:yyyy</returns>
        public override string ToString()
        {
            return Address.ToString() + ":" + PortNumber.ToString();
        }

        /// <summary>
        /// Standard-Konstruktor
        /// </summary>
        public ChatConnection() { }

        /// <summary>
        /// Parser-Konstruktor zum Einlesen von 
        /// gespeicherten Einstellungen
        /// </summary>
        /// <param name="source">Zeile aus Sicherungsdatei</param>
        public ChatConnection(string source)
        {
            string[] tokens = source.Split(new char[] { ':' }, 2);

            if (tokens.Count() == 2)
            {
                IPAddress.TryParse(tokens[0], out Address);
                int.TryParse(tokens[1], out PortNumber);
            }
        }
    }
}
