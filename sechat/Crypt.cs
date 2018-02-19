using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sechat
{
    /// <summary>
    /// Sammlung von Methoden zur Codierung und
    /// Verschlüsselung von Texten
    /// </summary>
    public abstract class Crypt
    {
        /// <summary>
        /// Verschlüsselt einen Text
        /// </summary>
        /// <param name="plainText">Zu verschlüsselnder Text</param>
        /// <param name="key">Schlüssel</param>
        /// <returns>Verschlüsselter Text</returns>
        public abstract string Encrypt(string plainText, string key);

        /// <summary>
        /// Entschlüsselt einen Text
        /// </summary>
        /// <param name="cipherText">Zu entschlüsselnder Text</param>
        /// <param name="key">Schlüssel</param>
        /// <returns>Entschlüsselter Text</returns>
        public abstract string Decrypt(string cipherText, string key);

        /// <summary>
        /// Verschlüsselungsfunktion eines Strings
        /// </summary>
        /// <param name="plainText">Ursprünglicher String</param>
        /// <param name="key">Anzuwendender Schlüssel</param>
        /// <returns>Ver-/Entschlüsselter Text</returns>
        protected abstract string DoCrypt(string plainText, string key);

        /// <summary>
        /// Codiert eine Text in Base64-Format
        /// </summary>
        /// <param name="plainText">Plaintext</param>
        /// <returns>Codierter Text</returns>
        protected string Base64Encode(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Decodiert einen Text aus dem Base64-Format
        /// </summary>
        /// <param name="base64Text">Eingabetext</param>
        /// <returns>Plaintext</returns>
        protected string Base64Decode(string base64Text)
        {
            byte[] base64TextBytes = Convert.FromBase64String(base64Text);
            return Encoding.UTF8.GetString(base64TextBytes);
        }
    }
}
