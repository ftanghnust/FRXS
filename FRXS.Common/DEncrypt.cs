using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FRXS.Common
{
    public class DEncrypt
    {
        private static DEncrypt _dencryptopt;
        public static DEncrypt DEncryptOpt
        {
            get
            {
                if(_dencryptopt==null)
                {
                    _dencryptopt=new DEncrypt();
                }

                return _dencryptopt;
            }
        }

        /// <summary>
        /// 登录cookie名称
        /// </summary>
        public const string VerifyCodeKey = "qadbA1mpYXw=";

        
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Md5Stirng(string value)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = Encoding.Default.GetBytes(value);
            byte[] result = md5.ComputeHash(data);
            string ret = "";
            for (int i = 0; i < result.Length; i++)
            {
                ret += result[i].ToString("x2");
            }
            return ret;
        }


        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="encryptString">要加密字符串</param>
        /// <param name="encryptKey">用于加密的key</param>
        /// <returns>加密后的字符串</returns>
        public string Encode(string encryptString, string encryptKey)
        {
            SymmetricAlgorithm rijndaelProvider = new DESCryptoServiceProvider();
            rijndaelProvider.Key = Convert.FromBase64String(encryptKey);
            rijndaelProvider.IV = Convert.FromBase64String(encryptKey);
            ICryptoTransform rijndaelEncrypt = rijndaelProvider.CreateEncryptor();

            byte[] inputData = Encoding.UTF8.GetBytes(encryptString);
            byte[] encryptedData = rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);

            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="decryptString">要解密的字符串</param>
        /// <param name="decryptKey">用于解密的key</param>
        /// <returns>解密后的字符串</returns>
        public string Decode(string decryptString, string decryptKey)
        {
            SymmetricAlgorithm rijndaelProvider = new DESCryptoServiceProvider();
            rijndaelProvider.Key = Convert.FromBase64String(decryptKey);
            rijndaelProvider.IV = Convert.FromBase64String(decryptKey);
            ICryptoTransform rijndaelDecrypt = rijndaelProvider.CreateDecryptor();

            byte[] inputData = Convert.FromBase64String(decryptString);
            byte[] decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);

            return Encoding.UTF8.GetString(decryptedData);
        }
    }
}
