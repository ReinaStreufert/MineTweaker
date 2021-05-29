using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker
{
    public static class CryptoUtils
    {
        public static RSAParameters ParamsFromDERKey(byte[] encodedKey)
        {
            AsymmetricKeyParameter asymmetricKeyParam = PublicKeyFactory.CreateKey(encodedKey);
            RsaKeyParameters bouncyCastleParams = (RsaKeyParameters)asymmetricKeyParam;
            RSAParameters sharpParams = new RSAParameters();
            sharpParams.Modulus = bouncyCastleParams.Modulus.ToByteArrayUnsigned();
            sharpParams.Exponent = bouncyCastleParams.Exponent.ToByteArrayUnsigned();
            return sharpParams;
        }
        public static string MinecraftShaDigest(string serverID, byte[] sharedSecret, byte[] publicKey)
        {
            byte[] serverHashBytes = Encoding.ASCII.GetBytes(serverID);
            byte[] input = new byte[sharedSecret.Length + publicKey.Length + serverHashBytes.Length];
            serverHashBytes.CopyTo(input, 0);
            sharedSecret.CopyTo(input, serverHashBytes.Length);
            publicKey.CopyTo(input, sharedSecret.Length + serverHashBytes.Length);
            var hash = new SHA1Managed().ComputeHash(input);
            // Reverse the bytes since BigInteger uses little endian
            Array.Reverse(hash);

            BigInteger b = new BigInteger(hash);
            // very annoyingly, BigInteger in C# tries to be smart and puts in
            // a leading 0 when formatting as a hex number to allow roundtripping 
            // of negative numbers, thus we have to trim it off.
            if (b < 0)
            {
                // toss in a negative sign if the interpreted number is negative
                return "-" + (-b).ToString("x").TrimStart('0');
            }
            else
            {
                return b.ToString("x").TrimStart('0');
            }
        }
        public static UserData GetUserDataFromLauncher()
        {
            UserData data = new UserData();
            try
            {
                string launcherAccountsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.minecraft\\launcher_accounts.json";
                if (File.Exists(launcherAccountsPath))
                {
                    string json = File.ReadAllText(launcherAccountsPath);
                    JObject mainObj = JObject.Parse(json);
                    string activeAccount = mainObj.GetValue("activeAccountLocalId").ToString();
                    JObject accounts = (JObject)mainObj.GetValue("accounts");
                    JObject account = (JObject)accounts.GetValue(activeAccount);
                    
                    data.AccessToken = account.GetValue("accessToken").ToString();

                    JObject profile = (JObject)account.GetValue("minecraftProfile");

                    data.UUID = profile.GetValue("id").ToString();
                    return data;
                }
                else
                {
                    return data;
                }
            } catch
            {
                return data;
            }
        }
    }
    public struct UserData
    {
        public string AccessToken;
        public string UUID;
    }
}
