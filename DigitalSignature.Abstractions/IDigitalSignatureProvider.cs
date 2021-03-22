using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalSignature.Abstractions
{
    public interface IDigitalSignatureProvider
    {
        /// <summary>
        /// 获取Key
        /// 键为公钥，值为私钥
        /// </summary>
        /// <returns></returns>
        KeyValuePair<string, string> CreateRSAKey();

        /// <summary>
        /// RSA实现数字签名
        /// </summary>
        /// <param name="plaintext">原文</param>
        /// <param name="privateKey">私钥</param>
        /// <returns>签名</returns>
        string HashAndSignString(string plaintext, string privateKey);

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="plaintext">原文</param>
        /// <param name="SignedData">签名</param>
        /// <param name="publicKey">公钥</param>
        /// <returns></returns>
        bool VerifySigned(string plaintext, string SignedData, string publicKey);
    }
}
