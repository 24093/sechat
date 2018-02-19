using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sechat
{
    /// <summary>
    /// ViewModel für ChatMessage-Objekt für die Darstellung von
    /// Anzeigeeigenschaften
    /// </summary>
    /// <see cref="ViewModelBase"/>
    /// <see cref="ChatMessage"/>
    public class ChatMessageViewModel : ViewModelBase<ChatMessage>
    {
        /// <summary>
        /// Schlüssel zur Decodierung der Nachricht
        /// </summary>
        string key;

        /// <summary>
        /// Konstruktor mit ChatMessage-Parameter
        /// </summary>
        /// <param name="source">ChatMessage-Model-Objekt</param>
        /// <param name="brush">Farbe des Textes</param>
        /// <param name="key">Schlüssel zur Anzeige der Nachricht</param>
        public ChatMessageViewModel(ChatMessage source, System.Windows.Media.Brush brush, string key)
        {
            this.Source = source;
            SenderTextBrush = brush;
            this.key = key;
        }
                
        /// <summary>
        /// Anzuzeigender Nachrichtentext
        /// </summary>
        public string MessageText 
        {
            get
            {
                return Source.PlainText;
            }         
        }

        /// <summary>
        /// Anzuzeigender Absendertext
        /// </summary>
        public string SenderText
        {
            get
            {
                return Source.Sender;
            }
        }

        /// <summary>
        /// Farbe, in der der Absendertext dargestellt werden soll
        /// </summary>
        private System.Windows.Media.Brush _senderTextBrush;

        /// <summary>
        /// Farbe, in der der Absendertext dargestellt werden soll
        /// </summary>
        public System.Windows.Media.Brush SenderTextBrush
        {
            get
            {
                return _senderTextBrush;
            }

            set
            {
                // Wert setzen und PropertyChanged-Event auslösen
                _senderTextBrush = value;
                OnPropertyChanged("SenderTextColor");
            }
        }

        /// <summary>
        /// Farbe, in der der Nachrichtentext dargestellt werden soll
        /// (Z.Zt in der gleichen Farbe wie der Absendertext)
        /// </summary>
        public System.Windows.Media.Brush MessageTextBrush
        {
            get
            {
                return SenderTextBrush;
            }
        }
    }
}
