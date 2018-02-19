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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace sechat
{
    /// <summary>
    /// Hauptfenster mit Chatfenster und Eingabebereich
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// ChatServer-Objekt zum Empfangen von Daten
        /// </summary>
        ChatServer chatServer = null;

        /// <summary>
        /// ChatClient-Object zum Senden von Daten
        /// </summary>
        ChatClient chatClient = null;

        /// <summary>
        /// Verbindungseinstellungen
        /// </summary>
        ConnectionSettings connectionSettings = new ConnectionSettings();

        /// <summary>
        /// Nachrichten, die seit dem Start des Programms
        /// gesendet und empfangen wurden
        /// </summary>
        ObservableCollection<ChatMessageViewModel> chatMessages = new ObservableCollection<ChatMessageViewModel>();

        /// <summary>
        /// Konstruktor mit Standard-Init
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handler für Loaded-Event beim Laden des Fensters
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // ItemSource für ListView festlegen
            ChatLogListView.ItemsSource = chatMessages;

            // Ggf. Einstellungen laden
            connectionSettings.Load();
        }

        /// <summary>
        /// Serverinitialisierung (Start des Servers und
        /// registrierung des Eventhandlers für Nachrichtenemfpang)
        /// </summary>
        /// <see cref="chatServer_MessageReceived"/>
        private void InitServer()
        {
            try
            {
                // ChatServer-Objekt erstellen
                chatServer = new ChatServer(connectionSettings.Key);

                // Eventhandler für Nachrichtenempfang registrieren
                chatServer.MessageReceived += new ChatServer.MessageReceivedEventHandler(chatServer_MessageReceived);
                
                // Server starten (serverConnection gibt IP-Adresse und insb. den Port vor)
                chatServer.BeginListen(connectionSettings.ServerConnection);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Fehler beim Inititalisieren des Servers", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Clientinitialisierung (Erstellung für eine
        /// bestimmte Verbindung)
        /// </summary>
        private void InitClient()
        {
            try
            {
                // ChatClient-Objekt erstellen für die in clientConnection definierte Verbindung
                chatClient = new ChatClient(connectionSettings.ClientConnection);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Fehler beim Inititalisieren des Clients", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Eventhandler für Nachrichtenempfang des Servers
        /// </summary>
        /// <param name="e">Ein MessageReceivedEventArgs-Objekt, das ein ChatMessage-Objekt enthält</param>
        /// <see cref="MessageReceivedEventArgs"/>
        /// <see cref="ChatMessage"/>
        private void chatServer_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            // Eigene Nachrichten nicht erneut anzeigen (bei MultiChatServer wird die eigene
            // Nachricht prinzipbedingt erneut an den Absender gesendet)
            if (e.Message.Sender != connectionSettings.UserName)
            {
                // Neue Nachricht im Log speichern            
                chatMessages.Add(new ChatMessageViewModel(e.Message, Brushes.Black, connectionSettings.Key));

                // Anzeige des ListView zur neuen Nachricht scollen
                ChatLogListView.ScrollIntoView(e.Message);
            }
        }

        /// <summary>
        /// Eventhandler für einen Klick auf den "Senden"-Button
        /// </summary>
        /// <see cref="ChatMessage"/>
        /// <see cref="ChatClient"/>
        private void ChatSendInputButton_Click(object sender, RoutedEventArgs e)
        {
            // Verbindung herstellen
            InitClient();

            // Fehler ausgeben, falls ChatClient-Objekt 
            // nicht initialisiert ist
            if (chatClient != null)
            {
                try
                {
                    // Nachricht in Form eines ChatMessage-Objekts erstellen
                    ChatMessage message = new ChatMessage(connectionSettings.UserName, connectionSettings.ServerConnection.PortNumber.ToString(), ChatInputTextBox.Text, connectionSettings.Key);
                    
                    // Nachricht über ChatClient senden
                    chatClient.Send(message);

                    // Nachricht im eigenen Chat-Log speichern
                    chatMessages.Add(new ChatMessageViewModel(message, Brushes.Gray, connectionSettings.Key));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Fehler beim Senden der Nachricht", MessageBoxButton.OK, MessageBoxImage.Error);
                }
				finally
				{				
                    // Eingabebereich leeren
                    ChatInputTextBox.Clear();
				}
            }
            else
            {
                MessageBox.Show(this, "Client nicht initialisiert." + Environment.NewLine + "Bitte Verbindungseinstellungen überprüfen.", 
                    "Fehler beim Senden der Nachricht", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Eventhandler für Klick auf Verbindungseinstellungen-Menüpunkt
        /// </summary>
        /// <see cref="InitServer"/>
        /// <see cref="InitClient"/>
        private void ConnectionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Einstellungsfenster erzeugen
            ConnectionWindow wnd = new ConnectionWindow(connectionSettings)
            {
                Owner = this,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner
            };

            // Bei "OK" Chatkomponenten initialisieren
            if (wnd.ShowDialog() == true)
            {
                // Server initialisieren
                InitServer();

                // Eingabebereich aktivieren
                ChatLogListView.IsEnabled = true;
                ChatInputTextBox.IsEnabled = true;
                ChatSendInputButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// Eventhandler für Klick auf Speichern-Menüpunkt
        /// </summary>
        private void SaveLogMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Dialog erzeugen
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.FileName = "chatlog";            
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = "Textdateien|*.txt";

            // Dialog anzeigen
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Nachrichten in Strings konvertieren
                    List<string> lines = new List<string>();

                    foreach (ChatMessageViewModel message in chatMessages)
                    {
                        lines.Add("<" + message.SenderText + "> " + message.MessageText);
                    }

                    // Strings in Datei schreiben
                    System.IO.File.WriteAllLines(saveFileDialog.FileName, lines, Encoding.UTF8);

                    // Erfolgsmeldung anzeigen
                    MessageBox.Show(this, "Die Nachrichten wurden erfolgreich gespeichert.", "Datei gespeichert", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Fehler beim Speichern", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Eventhandler für den Klick auf Servre-Menüpunkt
        /// </summary>
        private void RunServerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Server starten
                MultiChatServer mcs = new MultiChatServer();
                mcs.BeginListen(connectionSettings.ServerConnection);

                // Adresse anzeigen
                MessageBox.Show("Server aktiv an " + connectionSettings.ServerConnection.ToString() + "." + Environment.NewLine +
                    "OK zum Beenden.", "Server aktiv", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Nach Klick auf OK Programm schließen
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Fehler beim Inititalisieren des Servers", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
