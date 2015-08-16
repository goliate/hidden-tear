/*
 _     _     _     _              _                  
| |   (_)   | |   | |            | |                 
| |__  _  __| | __| | ___ _ __   | |_ ___  __ _ _ __ 
| '_ \| |/ _` |/ _` |/ _ \ '_ \  | __/ _ \/ _` | '__|
| | | | | (_| | (_| |  __/ | | | | ||  __/ (_| | |   
|_| |_|_|\__,_|\__,_|\___|_| |_|  \__\___|\__,_|_| 
 * Coded by Utku Sen(Jani) / August 2015 Istanbul / utkusen.com 
 * hidden tear may be used only for Educational Purposes. Do not use it as a ransomware!
 * You could go to jail on obstruction of justice charges just for running hidden tear, even though you are innocent.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace hidden_tear_decrypter
{
    public partial class Form1 : Form
    {
        string userName = Environment.UserName;
        string userDir = "C:\\Users\\";

        public Form1()
        {
            InitializeComponent();
        }

        public byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                     
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                        
                  
                }
            }

            return decryptedBytes;
        }

        public void DecryptFile(string file,string password)
        {

            byte[] bytesToBeDecrypted = File.ReadAllBytes(file);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            File.WriteAllBytes(file, bytesDecrypted);
            string extension = System.IO.Path.GetExtension(file);
            string result = file.Substring(0, file.Length - extension.Length);
            System.IO.File.Move(file, result);

        }

        public void DecryptDirectory(string location)
        {
            string password = textBox1.Text;

            string[] files = Directory.GetFiles(location);
            string[] childDirectories = Directory.GetDirectories(location);
            for (int i = 0; i < files.Length; i++)
            {
                string extension = Path.GetExtension(files[i]);
                if (extension == ".locked")
                {
                    DecryptFile(files[i], password);
                }
            }
            for (int i = 0; i < childDirectories.Length; i++)
            {
                DecryptDirectory(childDirectories[i]);
            }
            label3.Visible = true;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = "\\Desktop";
            string fullpath = userDir + userName + path;
            DecryptDirectory(fullpath);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
