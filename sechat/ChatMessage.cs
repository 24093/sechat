using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sechat
{
    /// <summary>
    /// Eine Nachricht im Chat-System bestehend 
    /// aus Absendername und Nachrichtentext
    /// </summary>
    public class ChatMessage
    {
        /// <summary>
        /// Absendername
        /// </summary>
        public string Sender { get; private set; }

        /// <summary>
        /// Portnummer als String
        /// </summary>
        public string PortNumber { get; private set; }

        /// <summary>
        /// Nachrichtentext
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Verwendete Verschlüsselungsroutine
        /// </summary>
        private Crypt crypt;

        /// <summary>
        /// Schlüssel zur Decodierung der Nachricht
        /// </summary>
        private string key;

        /// <summary>
        /// Entschlüsselter Nachrichtentext
        /// </summary>
        public string PlainText
        {
            get
            {
                return crypt.Decrypt(Text, key);
            }
        }

        /// <summary>
        /// Konstruktor ohne Parser (findet Verwendung beim
        /// Senden von Nachrichten)
        /// </summary>
        /// <param name="sender">Absendername</param>
        /// <param name="portNumber">Zu sendende Portnummer (üblicherweise die des eigenen Servers)</param>
        /// <param name="text">Nachrichtentext</param>
        /// <param name="key">Schlüssel zur Decodierung des Textes</param>
        public ChatMessage(string sender, string portNumber, string text, string key)
        {
            crypt = new XorCrypt();

            // Absendername und Nachrichtentext speichern
            Sender = sender;
            PortNumber = portNumber;
            Text = crypt.Encrypt(text, key);
            this.key = key;
        }

        /// <summary>
        /// Konstruktor mit Parser (findet Verwendung beim Empfang
        /// von Nachrichten)
        /// </summary>
        /// <param name="source">Empfangender String</param>
        /// <param name="key">Schlüssel zur Decodierung des Textes</param>
        public ChatMessage(string source, string key)
        {
            crypt = new XorCrypt();
            this.key = key;

            try
            {
                // String beim ersten Vorkommen von "|" teilen
                string[] tokens = source.Split(new char[] { '|' }, 3);

                // Absendername und Nachrichtentext festlegen
                if (tokens.Count() == 3)
                {
                    // Standardfall
                    Sender = tokens[0];
                    PortNumber = tokens[1];
                    Text = tokens[2];
                }
                else if (tokens.Count() == 1)
                {
                    // Fehlerfall bei fehlendem Trennzeichen
                    Sender = "unknown";
                    PortNumber = (31338).ToString();
                    Text = tokens[0];
                }
                else
                {
                    // Sonstiger Fehlerfall
                    throw new Exception("Fehler beim Parsen der Nachricht: " + source);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Fehler beim Parsen der Nachricht: " + source, ex);
            }
        }

        /// <summary>
        /// Umwandlung des Objekts in einen String
        /// inkl. Trennzeichen
        /// </summary>
        /// <returns>Nachricht im Sendeformat</returns>
        public string GetMessage()
        {
            return Sender + "|" + PortNumber + "|" + Text;
        }

    }
}
