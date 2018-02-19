using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace sechat
{
    /// <summary>
    /// Basisklasse für grundlegende ViewModels
    /// </summary>
    public class ViewModelBase<T> : INotifyPropertyChanged
    {
        /// <summary>
        /// Ursprüngliches Model-Objekt
        /// </summary>
        protected T Source { get; set; }

        /// <summary>
        /// Event zur Mitteilung von Eigenschaftsänderungen
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Methode zum Auslösen des Events bei 
        /// Eigenschaftsänderungen 
        /// </summary>
        /// <param name="propertyName">Name der geänderten Eigenschaft</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
