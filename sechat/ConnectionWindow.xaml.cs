using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;

namespace sechat
{
    /// <summary>
    /// Fenster zum festlegen der grundlegenden Verbindungseinstellungen
    /// </summary>
    public partial class ConnectionWindow : Window
    {
        /// <summary>
        /// Verbindungseinstllungen
        /// </summary>
        ConnectionSettings connectionSettings = null;
        
        /// <summary>
        /// Konstruktor mit im MainWindow erzeugten
        /// ChatConnection-Objekten
        /// </summary>
        /// <param name="settings">Verbindungseinstellungen</param>
        public ConnectionWindow(ConnectionSettings settings)
        {
            InitializeComponent();

            // Verbindungsdaten speichern
            connectionSettings = settings;
            
            // Namen erzeugen
            if (settings.UserName == null)
            {
                connectionSettings.UserName = GetRandomName();
            }
        }

        /// <summary>
        /// Erzeugt einen Zufallsnamen im Format Userxxxx
        /// </summary>
        /// <returns>Zufällig erzeugter Benutzername</returns>
        private string GetRandomName()
        {
            Random random = new Random();
            return "User" + random.Next(1000, 9999).ToString();
        }

        /// <summary>
        /// Eventhandler für das Laden den Fensters
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Server-Verbindung mit Vorgaben füllen
            ServerHostTextBox.Text = connectionSettings.ServerConnection.Address.ToString();
            ServerPortTextBox.Text = connectionSettings.ServerConnection.PortNumber.ToString();

            // Client-Verbindung mit Vorgaben füllen
            ClientHostTextBox.Text = connectionSettings.ClientConnection.Address.ToString();
            ClientPortTextBox.Text = connectionSettings.ClientConnection.PortNumber.ToString();

            // Name und Schlüssel
            NameTextBox.Text = connectionSettings.UserName;
            KeyTextBox.Text = connectionSettings.Key;
        }

        /// <summary>
        /// Parsermethode für eine Host-/Port-Textbox
        /// </summary>
        /// <param name="hostTextBox">Host-Textbox</param>
        /// <param name="portTextBox">Port-Textbox</param>
        /// <returns>ChatConnection bei Erfolg oder null bei Fehler</returns>
        private ChatConnection Parse(TextBox hostTextBox, TextBox portTextBox)
        {
            // Adresse parsen 
            IPAddress tempAddress = null;
            bool success = IPAddress.TryParse(hostTextBox.Text, out tempAddress);

            // Port parsen
            int tempPortNumber = 0;
            success &= int.TryParse(portTextBox.Text, out tempPortNumber);

            // ChatConnection-Objekt zurückgeben
            if (success)
            {
                return new ChatConnection 
                {
                    Address = tempAddress,
                    PortNumber = tempPortNumber 
                };
            }
            else
            {
                // Fehlerfall: null zurückgeben
                return null;
            }
        }

        /// <summary>
        /// Eventhandler für Klick auf den "OK"-Button
        /// </summary>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Server-Daten parsen und ggf. Default setzen
            ChatConnection tempServerConnection = Parse(ServerHostTextBox, ServerPortTextBox);

            // Server-Daten parsen und ggf. Default setzen
            ChatConnection tempClientConnection = Parse(ClientHostTextBox, ClientPortTextBox);

            // Eingaben übernehmen
            if (tempServerConnection == null || tempClientConnection == null)
            {
                // Fehler beim Parsen
                MessageBox.Show(this, "Beim Einlesen mindestens einer der Angaben ist ein Fehler aufgetreten." + Environment.NewLine + 
                    "Bitte die Schreibweise der IP-Adressen und Portnummern überprüfen.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // Server-Daten speichern
                connectionSettings.ServerConnection.Address = tempServerConnection.Address;
                connectionSettings.ServerConnection.PortNumber = tempServerConnection.PortNumber;

                // Client-Daten speichern
                connectionSettings.ClientConnection.Address = tempClientConnection.Address;
                connectionSettings.ClientConnection.PortNumber = tempClientConnection.PortNumber;

                // Name speichern (muss zwischen 3 und 8 Zeichen lang sein)
                if (NameTextBox.Text.Length > 8)
                {
                    connectionSettings.UserName = NameTextBox.Text.Substring(0, 8);
                }
                else if (NameTextBox.Text.Length < 3)
                {
                    connectionSettings.UserName = GetRandomName();
                }
                else
                {
                    connectionSettings.UserName = NameTextBox.Text;
                }

                // Schlüssel speichern
                connectionSettings.Key = KeyTextBox.Text;

                // Einstellungen speichern
                connectionSettings.Save();

                // Dialog-Result setzen und schließen
                DialogResult = true;
                Close();
            }
        }

        /// <summary>
        /// Eventhandler für den "Abbrechen"-Button
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Dialog-Result setzen und schließen
            DialogResult = false;
            Close();
        }
    }
}
