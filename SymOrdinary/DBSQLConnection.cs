using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SymOrdinary
{
   public class DBSQLConnection
    {
        public DBSQLConnection()
        {
        }

        public static class Converter
        {
            public static string DESDecrypt(string passPhrase, string IV, string dataToDecrypt)
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(passPhrase.Substring(0, 8));
                byte[] ivBytes = Encoding.UTF8.GetBytes(IV.Substring(0, 8));
                byte[] encryptedData = Convert.FromBase64String(dataToDecrypt);

                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(keyBytes, ivBytes), CryptoStreamMode.Write))
                        {
                            cs.Write(encryptedData, 0, encryptedData.Length);
                            cs.FlushFinalBlock();

                            return Encoding.UTF8.GetString(ms.ToArray());
                        }
                    }
                }
            }

            public static string DESEncrypt(string passPhrase, string IV, string dataToEncrypt)
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(passPhrase.Substring(0, 8));
                byte[] ivBytes = Encoding.UTF8.GetBytes(IV.Substring(0, 8));
                byte[] data = Encoding.UTF8.GetBytes(dataToEncrypt);

                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(keyBytes, ivBytes), CryptoStreamMode.Write))
                        {
                            cs.Write(data, 0, data.Length);
                            cs.FlushFinalBlock();

                            return Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }
        }

        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;

        public SqlConnection GetConnection()
        {

            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(appDirectory, "Files/SuperInformation.xml");
            XmlDocument doc = new XmlDocument();

            string sysUserName = string.Empty;
            string sysPassword = string.Empty;
            string sysDataSource = string.Empty;
            string DatabaseName = string.Empty;

            try
            {
                doc.Load(filePath);
                XmlNode superInfoNode = doc.SelectSingleNode("/Super/SuperInfo");

                sysUserName = Converter.DESDecrypt(PassPhrase, EnKey, superInfoNode.Attributes["tom"].Value);
                sysPassword = Converter.DESDecrypt(PassPhrase, EnKey, superInfoNode["jery"].InnerText);
                sysDataSource = Converter.DESDecrypt(PassPhrase, EnKey, superInfoNode["mini"].InnerText);
                DatabaseName = Converter.DESDecrypt(PassPhrase, EnKey, superInfoNode["doremon"].InnerText);

                string connectionString = "data source=" + sysDataSource + ";Initial catalog=" + DatabaseName + ";user id=" + sysUserName + ";password=" + sysPassword + ";Integrated Security=False;connect Timeout=600;MultipleActiveResultSets=True; pooling=no;";

                SqlConnection conn = new SqlConnection(connectionString);
                return conn;
            }
            catch (Exception ex)
            {
                return null;
            }      
        }
    }
}
