using DeviceId;
using IceCoffee.Common.Security.Cryptography;
using Microsoft.Owin.Security.DataProtection;
using System.Text;

namespace LSTY.Sdtd.ServerAdmin.WebApi.DataProtection
{
    internal class AesDataProtector : IDataProtector
    {
        // length: 16
        private static readonly byte[] _key;
        private static readonly byte[] _iv;

        static AesDataProtector()
        {
            var currentDeviceId = new DeviceIdBuilder()
                .AddMachineName()
                .AddUserName()
                .AddMacAddress()
                .AddOsVersion()
                .ToString();

            using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            _key = md5.ComputeHash(Encoding.UTF8.GetBytes(currentDeviceId));
            _iv = md5.ComputeHash(Encoding.UTF8.GetBytes(UnityEngine.Device.SystemInfo.deviceUniqueIdentifier));
        }

        public byte[] Protect(byte[] unprotectedData)
        {
            return AES.Encrypt(unprotectedData, _key, _iv);
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return AES.Decrypt(protectedData, _key, _iv);
        }
    }
}