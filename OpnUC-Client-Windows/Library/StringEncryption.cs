using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpnUC_Client_Windows.Library
{
    public class StringEncryption
    {

        /// <summary>
        /// パスワードから共有キーと初期化ベクタを作成
        /// </summary>
        /// <param name="password"></param>
        /// <param name="keySize"></param>
        /// <param name="key"></param>
        /// <param name="blockSize"></param>
        /// <param name="iv"></param>
        private static void GenerateKeyFromPassword(string password, int keySize, out byte[] key, int blockSize, out byte[] iv)
        {
            byte[] salt = System.Text.Encoding.UTF8.GetBytes(string.Format("{0}{1}", System.Environment.MachineName, System.Environment.UserName));
            Rfc2898DeriveBytes rdb = new Rfc2898DeriveBytes(password, salt);
            rdb.IterationCount = 999;  // 演算反復処理回数
            key = rdb.GetBytes(keySize / 8);
            iv = rdb.GetBytes(blockSize / 8);
        }

        /// <summary>
        /// 文字列を暗号化
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string EncryptString(string sourceString, string password = "HWujgjG4uAaiRy3t")
        {
            if (string.IsNullOrEmpty(sourceString))
            {
                return "";
            }

            RijndaelManaged rm = new RijndaelManaged();
            byte[] key, iv;
            GenerateKeyFromPassword(password, rm.KeySize, out key, rm.BlockSize, out iv);
            rm.Key = key;
            rm.IV = iv;

            byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(sourceString);
            string strRet;
            using (ICryptoTransform ict = rm.CreateEncryptor())
            {
                byte[] encBytes = ict.TransformFinalBlock(strBytes, 0, strBytes.Length);
                strRet = Convert.ToBase64String(encBytes);
            }
            return strRet;
        }

        /// <summary>
        /// 暗号化された文字列を復号化
        /// </summary>
        /// <param name="encryptString"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string DecryptString(string encryptString, string password = "HWujgjG4uAaiRy3t")
        {
            if (string.IsNullOrEmpty(encryptString))
            {
                return "";
            }

            RijndaelManaged rm = new RijndaelManaged();
            byte[] key, iv;
            GenerateKeyFromPassword(password, rm.KeySize, out key, rm.BlockSize, out iv);
            rm.Key = key;
            rm.IV = iv;

            byte[] strBytes = Convert.FromBase64String(encryptString);
            string strRet;
            using (ICryptoTransform ict = rm.CreateDecryptor())
            {
                byte[] decBytes = ict.TransformFinalBlock(strBytes, 0, strBytes.Length);
                strRet = System.Text.Encoding.UTF8.GetString(decBytes);
            }
            return strRet;
        }

    }
}
