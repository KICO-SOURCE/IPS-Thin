using System.Diagnostics;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Collections.Generic;

namespace Assets.CaseFile
{
    public class DataEncryption
    {
        private const string encryptionKey = "KICOKNEESYSTEMS3471#2001";
        internal bool DecryptEncryptFile(byte[] source, string target, string key = encryptionKey)
        {
            string currentkey;
            if (key != "") currentkey = key;
            else currentkey = encryptionKey;

            RijndaelManaged encryptionProvider = GetEncryptor(currentkey);
            ICryptoTransform decryptor = encryptionProvider.CreateDecryptor();

            var sourceStream = new MemoryStream(source);
            sourceStream.Seek(0, SeekOrigin.Begin);
            CryptoStream decryptionStream = new CryptoStream(sourceStream, decryptor, CryptoStreamMode.Read);
            try
            {
                byte[] sourceBytes = new byte[(int)sourceStream.Length];
                int decryptionLength = decryptionStream.Read(sourceBytes, 0, (int)sourceStream.Length);
                FileStream targetStream = new FileStream(target, FileMode.Create, FileAccess.Write);
                targetStream.Write(sourceBytes, 0, decryptionLength);
                targetStream.Flush();

                encryptionProvider.Clear();
                decryptionStream.Close();
                sourceStream.Close();
                targetStream.Close();
            }
            catch (Exception e)
            {
                encryptionProvider.Clear();
                decryptionStream.Flush();
                decryptionStream = null;
                sourceStream.Flush();
                sourceStream.Close();
                sourceStream = null;

                return false;
            }

            return true;
        }

        private RijndaelManaged GetEncryptor(string key = "", string salt = "kico")
        {
            Rfc2898DeriveBytes derivedKey = new Rfc2898DeriveBytes(key, Encoding.Unicode.GetBytes(salt));
            RijndaelManaged rijndaelCsp = new RijndaelManaged();

            ///jkh
            /// Cipher mode and block size changed to fall in line with AES-256 standards
            /// Cipher Block Chaining Mode
            /// 
            rijndaelCsp.KeySize = 256;
            rijndaelCsp.BlockSize = 128;
            rijndaelCsp.Mode = CipherMode.CBC;


            rijndaelCsp.Key = derivedKey.GetBytes(rijndaelCsp.KeySize / 8);
            rijndaelCsp.IV = derivedKey.GetBytes(rijndaelCsp.BlockSize / 8);
            return rijndaelCsp;
        }

        public byte[] EncryptData(byte[] input)
        {
            if (input == null) return null;

            try
            {


                MemoryStream MemStream = new MemoryStream();
                var aesAlg = GetEncryptor(encryptionKey);
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                CryptoStream cStream = new CryptoStream(MemStream, encryptor, CryptoStreamMode.Write);
                cStream.Write(input, 0, input.Length);
                cStream.Close();
                byte[] encryptedData = MemStream.ToArray();
                return encryptedData;
            }
            catch
            {

                return null;
            }

        }

        public byte[] GetBytesFromString(string str)
        {
            if (str == null) return null;
            System.Text.ASCIIEncoding encoded = new System.Text.ASCIIEncoding();
            return encoded.GetBytes(str);
        }

        public byte[] GetBytesFromString1(string str)
        {
            if (str == null) return null;
            System.Text.UTF8Encoding encoded = new System.Text.UTF8Encoding();
            return encoded.GetBytes(str);
        }

        public XmlDocument ReadXML(byte[] input)
        {
            if (input == null) return null;

            try
            {


                byte[] decrytedData = DeCryptData(input);

                XmlDocument xd = new XmlDocument();

                MemoryStream memory = new MemoryStream(decrytedData, 0, decrytedData.Length);
                XmlTextReader tr = new XmlTextReader(memory);

                xd.Load(tr);


                return xd;
            }
            catch
            {

                return null;
            }

        }

        public byte[] DeCryptData(byte[] input)
        {
            if (input == null) return null;

            try
            {
                MemoryStream MemStream = new MemoryStream();
                RijndaelManaged aesAlg = GetEncryptor(encryptionKey);
                ICryptoTransform decryptor = aesAlg.CreateDecryptor();
                CryptoStream cStream = new CryptoStream(MemStream, decryptor, CryptoStreamMode.Write);
                cStream.Write(input, 0, input.Length);
                cStream.Close();
                byte[] encryptedData = MemStream.ToArray();
                return encryptedData;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                return null;
            }
        }

        internal string GetStringFromByteArray(byte[] bytes)
        {
            string str;
            try
            {
                System.Text.ASCIIEncoding encoded = new System.Text.ASCIIEncoding();
                str = encoded.GetString(bytes);
                return str;
            }
            catch
            {
                return null;
            }


        }

        internal byte[] BuildXMLEncryptedByteArray(XmlDocument xml)
        {
            byte[] data;
            if (xml == null) return null;
            try
            {
                using (System.IO.MemoryStream MemStream = new MemoryStream())
                {
                    WriteXML(MemStream, xml);
                    data = MemStream.GetBuffer();
                }
                return data;
            }
            catch
            {
                return null;
            }
        }

        internal void WriteXML(Stream s, XmlDocument xml)
        {
            try
            {
                using (System.IO.MemoryStream ms = new MemoryStream())
                {
                    using (XmlTextWriter xw = new XmlTextWriter(ms, System.Text.Encoding.UTF8))
                    {
                        xml.WriteContentTo(xw);
                        xw.Flush();
                        ms.Flush();
                    }
                    byte[] encrypted = EncryptData(ms.GetBuffer());
                    s.Write(encrypted, 0, encrypted.Length);
                    s.Flush();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<String> GetStringListFromXMLdoc(XmlDocument doc)
        {
            List<String> strl = new List<String>();

            XmlReader xmlReader = new XmlNodeReader(doc);
            while (!xmlReader.EOF)
            {
                if (xmlReader.MoveToContent() == XmlNodeType.Element && xmlReader.Name == "String")
                {
                    strl.Add(xmlReader.GetAttribute("str"));
                }

                if (!xmlReader.Read()) break;
            }

            xmlReader.Close();

            return strl;
        }

        internal bool EncryptKicoFile(string source, string target, bool clearsource)
        {
            try
            {


                RijndaelManaged encryptionProvider = GetEncryptor(encryptionKey);
                ICryptoTransform encryptor = encryptionProvider.CreateEncryptor();

                FileStream sourceStream = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                byte[] sourceBytes = new byte[(int)sourceStream.Length];
                sourceStream.Read(sourceBytes, 0, (int)sourceStream.Length);
                FileStream targetStream = new FileStream(target, FileMode.Create, FileAccess.Write);
                CryptoStream encryptionStream = new CryptoStream(targetStream, encryptor, CryptoStreamMode.Write);
                encryptionStream.Write(sourceBytes, 0, (int)sourceStream.Length);
                encryptionStream.FlushFinalBlock();

                encryptionProvider.Clear();
                encryptionStream.Close();
                sourceStream.Close();
                targetStream.Close();

                if (clearsource) System.IO.File.Delete(source);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                return false;
            }

            return true;

        }

        internal string EncryptString(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;

            var aesAlg = GetEncryptor(encryptionKey);
            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(text);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        internal bool DecryptencryptFile(string source, string target, string key)
        {
            string currentkey;
            if (key != "") currentkey = key;
            else currentkey = encryptionKey;
            RijndaelManaged encryptionProvider = GetEncryptor(currentkey);
            ICryptoTransform decryptor = encryptionProvider.CreateDecryptor();
            FileStream sourceStream = new FileStream(source, FileMode.Open, FileAccess.Read);
            CryptoStream decryptionStream = new CryptoStream(sourceStream, decryptor, CryptoStreamMode.Read);
            try
            {
                byte[] sourceBytes = new byte[(int)sourceStream.Length];
                int decryptionLength = decryptionStream.Read(sourceBytes, 0, (int)sourceStream.Length);
                FileStream targetStream = new FileStream(target, FileMode.Create, FileAccess.Write);
                targetStream.Write(sourceBytes, 0, decryptionLength);
                targetStream.Flush();
                encryptionProvider.Clear();
                decryptionStream.Close();
                sourceStream.Close();
                targetStream.Close();
            }
            catch
            {
                encryptionProvider.Clear();
                decryptionStream.Flush();
                decryptionStream = null;
                sourceStream.Flush();
                sourceStream.Close();
                sourceStream = null;
                return false;
             }
            return true;
        }

    }
}