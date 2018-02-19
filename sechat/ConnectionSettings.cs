using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace sechat
{
    /// <summary>
    /// Verbindungseinstellungen des Programms
    /// </summary>
    public class ConnectionSettings
    {
        /// <summary>
        /// Einstellungen zum Empfangen von Nachrichten
        /// </summary>
        public ChatConnection ServerConnection = new ChatConnection();

        /// <summary>
        /// Einstellungen zum Senden von Nachrichten
        /// </summary>
        public ChatConnection ClientConnection = new ChatConnection();

        /// <summary>
        /// Gewählter Benutzername
        /// </summary>
        public string UserName = null;

        /// <summary>
        /// Gewählter Schlüssel für Verschlüsselung
        /// </summary>
        public string Key = "sechat";

        /// <summary>
        /// Speichert die Einstellungen in eine Datei
        /// </summary>
        /// <param name="filename">Dateiname</param>
        public void Save(string filename = "settings.txt")
        {
            List<string> lines = new List<string>();
            lines.Add(ServerConnection.ToString());
            lines.Add(ClientConnection.ToString());
            lines.Add(UserName);
            lines.Add(Key);

            File.WriteAllLines(filename, lines, Encoding.UTF8);
        }

        /// <summary>
        /// Liest die Einstellungen aus einer Datei
        /// </summary>
        /// <param name="filename">Dateiname</param>
        public void Load(string filename = "settings.txt")
        {
            // Überprüfen, ob Datei vorhanden ist
            if (File.Exists(filename))
            {
                List<string> lines = File.ReadAllLines(filename, Encoding.UTF8).ToList();

                // Nur bei wohldefinierter Datei Versuch unternehmen, zu lesen
                if (lines.Count == 4)
                {
                    ServerConnection = new ChatConnection(lines[0]);
                    ClientConnection = new ChatConnection(lines[1]);

                    if (lines[2].Length > 2 && lines[2].Length < 9)
                    {
                        UserName = lines[2];
                    }

                    Key = lines[3];
                }
            }
        }
    }
}
