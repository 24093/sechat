using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sechat
{
    /// <summary>
    /// Implementierung eines einfachen Verschlüsselungsferfahrens
    /// mittels XOR
    /// </summary>
    public class XorCrypt : Crypt
    {
        /// <summary>
        /// Verschlüsselt einen Text
        /// </summary>
        /// <param name="plainText">Zu verschlüsselnder Text</param>
        /// <param name="key">Schlüssel</param>
        /// <returns>Verschlüsselter Text</returns>
        public override string Encrypt(string plainText, string key)
        {
            return Base64Encode(DoCrypt(plainText, key));
        }

        /// <summary>
        /// Entschlüsselt einen Text
        /// </summary>
        /// <param name="cipherText">Zu entschlüsselnder Text</param>
        /// <param name="key">Schlüssel</param>
        /// <returns>Entschlüsselter Text</returns>
        public override string Decrypt(string cipherText, string key)
        {
            return DoCrypt(Base64Decode(cipherText), key);
        }

        /// <summary>
        /// Verschlüsselungsfunktion für einfache
        /// XOR-Verschlüsselung eines Strings
        /// </summary>
        /// <param name="plainText">Ursprünglicher String</param>
        /// <param name="key">Anzuwendender Schlüssel</param>
        /// <returns>Ver-/Entschlüsselter Text</returns>
        protected override string DoCrypt(string plainText, string key)
        {
            StringBuilder cipherText = new StringBuilder();

            for (int c = 0; c < plainText.Length; c++)
            {
                cipherText.Append((char)((uint)plainText[c] ^ (uint)key[c % key.Length]));
            }

            return cipherText.ToString();
        }
    }
}
