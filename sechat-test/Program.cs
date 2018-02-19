using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sechat_test
{
    /// <summary>
    /// Rudimentärer Test
    /// im CTest-Stil
    /// </summary>
    class Program
    {		
        /// <summary>
        /// Testfunktion für Server-Client-Kommunkation insgesamt
        /// </summary>
        /// <returns>Anzahl Fehler</returns>
        static int TestServerClientCommunication()
        {
            int errorCount = 0;
			System.Net.IPAddress address = System.Net.IPAddress.Loopback;
			int portNumber = 31350;
			
			// ChatServer-Objekt erzeugen und Eventhandler definieren
            sechat.ChatServer cs = new sechat.ChatServer("key123");
            cs.MessageReceived += new sechat.ChatServer.MessageReceivedEventHandler((sechat.ChatServer sender, sechat.MessageReceivedEventArgs e) =>
            {
                if (e.Message.Sender != "alex")
                {
                    errorCount++;
                }

                if (e.Message.Text != "hello world")
                {
                    errorCount++;
                }
            });
			
			// ChatClient-Objekt erzeugen
            sechat.ChatClient cc = new sechat.ChatClient(new sechat.ChatConnection 
			{ 
				Address = address, 
				PortNumber = portNumber 
			});

			// ChatServer starten
            cs.BeginListen(new sechat.ChatConnection 
			{ 
				Address = address, 
				PortNumber = portNumber 
			});
			
			// Nachricht senden
            cc.Send(new sechat.ChatMessage("alex", "31338", "hello world", "key123"));

            return errorCount;
        }

        /// <summary>
        /// Testfunktion für das Parsen von ChatMessages
        /// </summary>
        /// <returns>Anzahl Fehler</returns>
        static int TestChatMessageParse()
        {
            int errorCount = 0;
			
			// Test mit vollständiger Nachricht
			// (Exception sollte nicht geworfen werden)
            try
            {
                sechat.ChatMessage msg = new sechat.ChatMessage("alex", "31338", "hello world", "AFS3gFKs=dW#$WuD-Rj$");
            }
            catch (Exception)
            {
                errorCount++;
            }

			// Test mit fehlerhafter Nachricht
			// (Exception sollte nicht geworfen werden)
            try
            {
                sechat.ChatMessage msg = new sechat.ChatMessage("hello world", "AFS3gFKs=dW#$WuD-Rj$");
            }
            catch (Exception)
            {
                errorCount++;
            }

			// Test mit null-String
			// (Exception sollte geworfen werden)
            bool exception_thrown = false;
			
            try
            {
                sechat.ChatMessage msg = new sechat.ChatMessage(null, "AFS3gFKs=dW#$WuD-Rj$");
            }
            catch (Exception)
            {
                exception_thrown = true;
            }

            if (!exception_thrown)
            {
                errorCount++;
            }

            return errorCount;
        }

        /// <summary>
        /// Testfunktion für die Verschlüsselung von Texten
        /// </summary>
        /// <returns>Anzahl Fehler</returns>
        static int TestEncryption()
        {
            int errorCount = 0;

            // Test XOR-Verschlüsselung
            string plainText = "Cthulhu fhtagn";
            sechat.Crypt crypt = new sechat.XorCrypt();

            string cipherText = crypt.Encrypt(plainText, "AFS3gFKs=dW#$WuD-Rj$");
            
            if (cipherText == plainText)
            {
                errorCount++;
            }

            string decipherText = crypt.Decrypt(cipherText, "AFS3gFKs=dW#$WuD-Rj$");

            if (decipherText != plainText)
            {
                errorCount++;
            }

            return errorCount;
        }

        static void Main(string[] args)
        {
            int errorSum = 0;

            int errorCount = TestServerClientCommunication();
            Console.WriteLine("TestServerClientCommunication     " + errorCount);
            errorSum += errorCount;

            errorCount = TestChatMessageParse();
            Console.WriteLine("TestChatMessageParse              " + errorCount);
            errorSum += errorCount;

            errorCount = TestEncryption();
            Console.WriteLine("TestEncryption                    " + errorCount);
            errorSum += errorCount;

            Console.WriteLine();
            Console.WriteLine("Total errors                      " + errorSum);
            Console.ReadKey();
        }

    }
}
