using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace WSPTools.BaseLibrary.Security
{
    public class StrongNameKey
    {
        #region From http://staceyw.spaces.live.com/Blog/cns!1pnsZpX0fPvDxLKC6rAAhLsQ!284.entry

        private const int magic_priv_idx = 0x08;
        private const int magic_pub_idx = 0x14;
        private const int magic_size = 4;


        /// <summary>
        /// Returns RSAParameters from byte[].
        /// Example to get rsa public key from assembly:
        /// byte[] pubkey = System.Reflection.Assembly.GetExecutingAssembly().GetName().GetPublicKey();
        /// RSAParameters p = SnkUtil.GetRSAParameters(pubkey); 
        /// </summary>
        /// <param name="keypair"></param>
        /// <returns></returns>
        private RSAParameters GetRSAParameters(byte[] keyBytes)
        {
            RSAParameters ret = new RSAParameters();

            if ((keyBytes == null) || (keyBytes.Length < 1))
                throw new ArgumentNullException("keyBytes");

            bool pubonly = SnkBufIsPubLength(keyBytes);

            if ((pubonly) && (!CheckRSA1(keyBytes)))
                return ret;

            if ((!pubonly) && (!CheckRSA2(keyBytes)))
                return ret;

            int magic_idx = pubonly ? magic_pub_idx : magic_priv_idx;

            // Bitlen is stored here, but note this 
            // class is only set up for 1024 bit length keys 
            int bitlen_idx = magic_idx + magic_size;
            int bitlen_size = 4;  // DWORD 

            // Exponent 
            // In read file, will usually be { 1, 0, 1, 0 } or 65537
            int exp_idx = bitlen_idx + bitlen_size;
            int exp_size = 4;


            //BYTE modulus[rsapubkey.bitlen/8]; == MOD; Size 128 
            int mod_idx = exp_idx + exp_size;
            int mod_size = 128;

            //BYTE prime1[rsapubkey.bitlen/16]; == P; Size 64 
            int p_idx = mod_idx + mod_size;
            int p_size = 64;

            //BYTE prime2[rsapubkey.bitlen/16]; == Q; Size 64   
            int q_idx = p_idx + p_size;
            int q_size = 64;

            //BYTE exponent1[rsapubkey.bitlen/16]; == DP; Size 64
            int dp_idx = q_idx + q_size;
            int dp_size = 64;

            //BYTE exponent2[rsapubkey.bitlen/16]; == DQ; Size 64   
            int dq_idx = dp_idx + dp_size;
            int dq_size = 64;

            //BYTE coefficient[rsapubkey.bitlen/16]; == InverseQ; Size 64
            int invq_idx = dq_idx + dq_size;
            int invq_size = 64;

            //BYTE privateExponent[rsapubkey.bitlen/8]; == D; Size 128 
            int d_idx = invq_idx + invq_size;
            int d_size = 128;


            // Figure public params 
            // Must reverse order (little vs. big endian issue)
            ret.Exponent = BlockCopy(keyBytes, exp_idx, exp_size);
            Array.Reverse(ret.Exponent);
            ret.Modulus = BlockCopy(keyBytes, mod_idx, mod_size);
            Array.Reverse(ret.Modulus);

            if (pubonly) return ret;

            // Figure private params    
            // Must reverse order (little vs. big endian issue)
            ret.P = BlockCopy(keyBytes, p_idx, p_size);
            Array.Reverse(ret.P);

            ret.Q = BlockCopy(keyBytes, q_idx, q_size);
            Array.Reverse(ret.Q);

            ret.DP = BlockCopy(keyBytes, dp_idx, dp_size);
            Array.Reverse(ret.DP);

            ret.DQ = BlockCopy(keyBytes, dq_idx, dq_size);
            Array.Reverse(ret.DQ);

            ret.InverseQ = BlockCopy(keyBytes, invq_idx, invq_size);
            Array.Reverse(ret.InverseQ);

            ret.D = BlockCopy(keyBytes, d_idx, d_size);
            Array.Reverse(ret.D);

            return ret;
        }

        private byte[] GetFileBytes(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                byte[] bytes = br.ReadBytes((int)fs.Length);
                return bytes;
            }
        }


        private byte[] BlockCopy(byte[] source, int idx, int size)
        {
            if ((source == null) || (source.Length < (idx + size)))
                return null;

            byte[] ret = new byte[size];
            Buffer.BlockCopy(source, idx, ret, 0, size);
            return ret;
        }

        /// <summary>
        /// Returns true if buffer length is public key size.
        /// </summary>
        /// <param name="keypair"></param>
        /// <returns></returns>
        private bool SnkBufIsPubLength(byte[] keypair)
        {
            if (keypair == null)
                return false;
            return (keypair.Length == 160);
        }

        /// <summary>
        /// Check that RSA1 is in header (public key only).
        /// </summary>
        /// <param name="keypair"></param>
        /// <returns></returns>
        private bool CheckRSA1(byte[] pubkey)
        {
            // Check that RSA1 is in header.
            //                             R     S     A     1 
            byte[] check = new byte[] { 0x52, 0x53, 0x41, 0x31 };
            return CheckMagic(pubkey, check, magic_pub_idx);
        }

        /// <summary>
        /// Check that RSA2 is in header (public and private key).
        /// </summary>
        /// <param name="keypair"></param>
        /// <returns></returns>
        private bool CheckRSA2(byte[] pubkey)
        {
            // Check that RSA2 is in header.
            //                             R     S     A     2 
            byte[] check = new byte[] { 0x52, 0x53, 0x41, 0x32 };
            return CheckMagic(pubkey, check, magic_priv_idx);
        }

        private bool CheckMagic(byte[] keypair, byte[] check, int idx)
        {
            byte[] magic = BlockCopy(keypair, idx, magic_size);
            if (magic == null)
                return false;

            for (int i = 0; i < magic_size; i++)
            {
                if (check[i] != magic[i])
                    return false;
            }

            return true;
        }
        #endregion


        /// <summary>
        /// from http://www.vsj.co.uk/articles/display.asp?id=590
        /// </summary>
        /// <param name="rsaParameters"></param>
        /// <returns></returns>
        public static UInt64 GetStrongNameToken(RSAParameters rsaParameters)
        {
            MemoryStream msi = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msi);
            bw.Write((UInt32)0x2400);
            bw.Write((UInt32)0x8004);
            bw.Write(rsaParameters.Modulus.Length + (4 * 5));
            bw.Write((byte)0x06);
            bw.Write((byte)0x02);
            bw.Write((UInt16)0x0000);
            bw.Write((UInt32)0x2400);
            string magic = "RSA1";
            bw.Write(magic.ToCharArray());
            bw.Write((UInt32)(rsaParameters.Modulus.Length * 8));
            // Note that externally generated RSAParameters may
            // have missing leading 0 bytes!
            byte[] e = (byte[])rsaParameters.Exponent.Clone();
            Array.Reverse(e);
            byte[] rightlength_e = new byte[] { 0, 0, 0, 0 };
            e.CopyTo(rightlength_e, 0);
            bw.Write(rightlength_e);
            byte[] m = (byte[])rsaParameters.Modulus.Clone();
            Array.Reverse(m);
            bw.Write(m);
            SHA1 crypto = new SHA1CryptoServiceProvider();
            byte[] hash = crypto.ComputeHash(msi.ToArray());
            msi.Close();
            MemoryStream mso = new MemoryStream(hash);
            mso.Seek(-8, SeekOrigin.End);
            BinaryReader br = new BinaryReader(mso);
            UInt64 token = br.ReadUInt64();
            mso.Close();
            return token;
        }

        /// <summary>
        /// Generating a strong name key pair file programmatically
        /// from http://www.stackenbloggen.de/PermaLink,guid,d106bd87-1718-4ee6-94f8-ac2644c9196a.aspx
        /// </summary>
        /// <param name="fileName">Fullname of the keyfile, which will be generated.</param>
        /// <param name="keySize">RSA key size. Default is 1024. Range is 384-16384 in 8 bit increments.</param>
        public static string CreateKeyPairFile(string fileName, int keySize)
        {
            if ((keySize % 8) != 0)
            {
                throw new CryptographicException("Invalid key size. Valid size is 384 to 16384 mod 8.  Default 1024.");
            }

            CspParameters parms = new CspParameters();
            parms.KeyNumber = 2;
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(keySize, parms);
            byte[] array = provider.ExportCspBlob(!provider.PublicOnly);

            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(array, 0, array.Length);
            }
            return string.Format("{0:x16}", GetStrongNameToken(provider.ExportParameters(true)));
        }



        public static string GetPublicKeyToken(string path)
        {
            string result = null;
            if (File.Exists(path))
            {
                byte[] bytes;

                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        bytes = br.ReadBytes((int)fs.Length);
                    }
                }

                StrongNameKey nameKey = new StrongNameKey();
                RSAParameters param = nameKey.GetRSAParameters(bytes);

                result = string.Format("{0:x16}", GetStrongNameToken(param));
            }

            return result;
        }

    }
}
