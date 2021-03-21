/*

* 数字签名的工作方法：

* 报文的发送方从报文文本中生成一个散列值（或报文摘要）。

* 发送方用自己的私人密钥对这个散列值进行加密来形成发送方的数字签名。

* 然后，这个数字签名将作为报文的附件和报文一起发送给报文的接收方。

* 报文的接收方首先从接收到的原始报文中计算出散列值（或报文摘要），

* 接着再用发送方的公用密钥来对报文附加的数字签名进行解密。

* 如果两个散列值相同、那么接收方就能确认该数字签名是发送方的。

* 通过数字签名能够实现对原始报文的鉴别。

*/

using System;

using System.Text;

using System.Security.Cryptography;

namespace DigitalSignature
{
    /*
     * 此算法架构：RSA + SHA1
    */
    //信息和签名的封包
    public struct DS
    {
        public byte[] data;
        public byte[] signature;
    }

    class Program
    {
        static DSACryptoServiceProvider dsa = new DSACryptoServiceProvider(); //创建了公钥和私钥对
        //创建数字签名
        DS CreateSignature(string strData)
        {
            SHA1Managed sha1 = new SHA1Managed();
            DSASignatureFormatter sigFormatter = new DSASignatureFormatter(dsa);
            byte[] data_Bytes = Encoding.ASCII.GetBytes(strData);
            byte[] hash_Bytes = sha1.ComputeHash(data_Bytes);
            sigFormatter.SetHashAlgorithm("SHA1");
            byte[] signedHash = sigFormatter.CreateSignature(hash_Bytes);
            DS ds = new DS();
            ds.data = hash_Bytes;
            ds.signature = signedHash;
            return ds;
        }

        //验证数字签名
        bool VerifySignature(DS ds)
        {
            byte[] remote_HashedValue = ds.data;
            byte[] remote_SignedHash = ds.signature;
            DSASignatureDeformatter sigDeformatter = new DSASignatureDeformatter(dsa);
            sigDeformatter.SetHashAlgorithm("SHA1");
            if (sigDeformatter.VerifySignature(remote_HashedValue, remote_SignedHash))
            {
                return true;
            }
            return false;

        }

        static void Main(string[] args)

        {
            string strMsg = "JGLRIEHDKVJFLHGJGYRKPYEVCNADWQKGLBUFOWDT"; //报文文本
            DS ds = new DS();
            Program prog = new Program();
            ds = prog.CreateSignature(strMsg);
            //模拟签名在传输途中遭到破坏或修改

            //...

            //ds.signature = new byte[40];
            if (prog.VerifySignature(ds))
            {
                //验证通过
                Console.WriteLine("The signature used to sign the hash has been verified.");
            }
            else
            {
                //验证未通过
                Console.WriteLine("The signature used to sign the hash doesn't match the hash.");
            } 
            Console.ReadLine();
        }
    }
}


