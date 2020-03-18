//-----------------------------------------------------------------------
// <copyright company="your company" file="XmlHelper.cs">
//  Copyright (c)  V1.0.0.0  
//  builder  name: arson
//  build    time: 2020-03-10
//  function desc: xml helper class
//  history  list:
//           2020-03-10 arison build the xml helper class!
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace NET2CommonHelper
{
    /// <summary>
    /// 安全相关操作帮助类
    /// </summary>
    /// <remarks>密码加密，解密，MD5加密等</remarks>
    public class SecurityHelper
    {
        /// </summary>
        ///  DES加密字符串
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public  string EncryptDES(string encryptString, string encryptKey)
        {
            byte[] rgbKey = Encoding.ASCII.GetBytes(encryptKey.Substring(0, 8));
            byte[] rgbIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            dCSP.Key = rgbKey;
            dCSP.IV = rgbIV;
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());
        }

        /// <summary>
        /// 解密DES字符串
        /// </summary>
        /// <param name="decryptString">解密字符串</param>
        /// <param name="decryptKey">密钥</param>
        /// <returns></returns>
        public  string DecryptDES(string decryptString, string decryptKey)
        {
            byte[] rgbKey = Encoding.ASCII.GetBytes(decryptKey.Substring(0, 8));
            byte[] rgbIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            byte[] inputByteArray = Convert.FromBase64String(decryptString);
            DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
            DCSP.Key = rgbKey;
            DCSP.IV = rgbIV;
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }

        /// <summary>
        /// 加密单字符
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public  string Encrypt(string Source)
        {
            byte[] bts = Encoding.ASCII.GetBytes(Source);
            for (int i = 0; i < bts.Length; i++)
            {
                bts[i]++;
            }
            return Convert.ToBase64String(bts);
        }


        /// <summary>
        /// 32位md5加密
        /// </summary>
        /// <param name="text">待加密明文</param>
        /// <param name="isLower">是否小写，默认小写</param>
        /// <returns>密文</returns>
        public  string MD5(string text, bool isLower = true)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
            bytes = md5.ComputeHash(bytes);
            md5.Clear();

            string ret = string.Empty;
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
            }

            return isLower ? ret.PadLeft(32, '0').ToLower() : ret.PadLeft(32, '0').ToUpper();
        }
    }
}
