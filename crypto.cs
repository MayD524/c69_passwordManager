
using System.Security.Cryptography;
using System.Text;
using System;

namespace c69_passwordManager.cryptography {

    public class Cryptography {
        private string defaultFile = "./database/secret.key";
        public const int AES_MIN_KEY_SIZE = 16;
        public const int AES_MAX_KEY_SIZE = 32;
        public Dictionary<string, string> cachedKeys = new Dictionary<string, string>();

        public string secretKey = string.Empty;
        private string formSecretKey(int length=AES_MIN_KEY_SIZE) {
            if (length < AES_MIN_KEY_SIZE) {
                length = AES_MIN_KEY_SIZE;
            }
            Random rand = new Random();
            string result = "";
            for (int i = 0; i < length; i++) {
                int chr = rand.Next(32, 127);
                // convert character to char
                result += Convert.ToChar(chr);
            }
            return result;
        }

        private string padding(string key) {
            if (key.Length > AES_MAX_KEY_SIZE) {
                return key.Substring(0, AES_MAX_KEY_SIZE - 1);
            }
            // fill the end of the string with 'O'
            for (int i = key.Length; i < AES_MIN_KEY_SIZE; i++) {
                key += (char) 69;
            }
            return key;
        }

        private string getSecretKey() {
            // check if file exists
            if (File.Exists(this.defaultFile)) {
                // read the file
                string keyEncoded = File.ReadAllText(this.defaultFile);
                return this.decrypt(keyEncoded, padding(Environment.MachineName)).Trim();
            }
            return formSecretKey(AES_MIN_KEY_SIZE);
        }

        public void saveKey() {
            File.WriteAllText(this.defaultFile, this.encrypt(this.secretKey, padding(Environment.MachineName)));
        }

        public string decrypt(string cipherText, string key) {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            //Console.WriteLine(this.secretKey == string.Empty ? Environment.MachineName : this.secretKey);
            using (Aes aes = Aes.Create()) {
                aes.Padding = PaddingMode.Zeros;
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer)) {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, decryptor, CryptoStreamMode.Read)) {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream)) {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public string encrypt(string plainText, string key) {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create()) {
                aes.Padding = PaddingMode.Zeros;
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream()) {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write)) {
                        using (StreamWriter stream = new StreamWriter((Stream)cryptoStream)) {
                            stream.Write(plainText);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(array);
        }

        public Cryptography() {
            if (File.Exists(this.defaultFile)) {
                this.secretKey = this.getSecretKey();
            } else {
                this.secretKey = this.formSecretKey();
            }
        }
    }
}