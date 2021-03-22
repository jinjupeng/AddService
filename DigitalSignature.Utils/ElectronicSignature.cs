using System;

namespace DigitalSignature.Utils
{
    public class ElectronicSignature
    {
        /// <summary>
        /// 签名规则
        /// </summary>
        public string SignatureRiles { get; set; }

        /// <summary>
        /// 签名时间
        /// </summary>
        public DateTime SignatureTime { get; set; }

        /// <summary>
        /// 签名人
        /// </summary>
        public string Signer { get; set; }

        /// <summary>
        /// 签名结果
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// 签名证书
        /// </summary>
        public string Certficate { get; set; }

        /// <summary>
        /// 证书引证
        /// 指向验证签名证书的链接
        /// </summary>
        public string CertficateReference { get; set; }

        /// <summary>
        /// 签名算法标识
        /// </summary>
        public string SignatureAlgorithmIdentifier { get; set; }
    }
}
