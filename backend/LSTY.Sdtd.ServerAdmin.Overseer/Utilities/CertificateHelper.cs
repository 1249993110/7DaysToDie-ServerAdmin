using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Operators;

namespace LSTY.Sdtd.ServerAdmin.Overseer.Utilities
{
    /// <summary>
    /// Helper class for managing certificates.
    /// </summary>
    public static class CertificateHelper
    {
        ///// <summary>
        ///// Generates a self-signed certificate with the specified name, key size, and validity period.
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="keySize"></param>
        ///// <param name="validYears"></param>
        ///// <returns></returns>
        //public static X509Certificate2 GenerateSignedCertificate(string name, int keySize = 2048, int validYears = 10)
        //{
        //    using var rsa = RSA.Create(keySize);
        //    var req = new CertificateRequest($"CN={name}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        //    req.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
        //    req.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, false));
        //    req.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(req.PublicKey, false));

        //    DateTimeOffset now = DateTimeOffset.UtcNow;
        //    return req.CreateSelfSigned(now.AddDays(-1), now.AddYears(validYears));
        //}

        ///// <summary>
        ///// Saves the certificate to a PFX file.
        ///// </summary>
        ///// <param name="cert"></param>
        ///// <param name="path"></param>
        ///// <param name="password"></param>
        //public static void SavePfx(X509Certificate2 cert, string path, string? password = null)
        //{
        //    if (password == null)
        //    {
        //        File.WriteAllBytes(path, cert.Export(X509ContentType.Pfx));
        //    }
        //    else
        //    {
        //        File.WriteAllBytes(path, cert.Export(X509ContentType.Pfx, password));
        //    }
        //}

        public static void GenerateSignedCertificate(string path, string subjectName, string? password = null, int keySize = 2048, int validYears = 10)
        {
            // 1. Generate RSA Key Pair
            var keyGen = new RsaKeyPairGenerator();
            keyGen.Init(new KeyGenerationParameters(new SecureRandom(), keySize));
            var keyPair = keyGen.GenerateKeyPair();

            // 2. Build Certificate Information
            var certGen = new X509V3CertificateGenerator();
            var certName = new X509Name($"CN={subjectName}");
            var serial = BigInteger.ProbablePrime(120, new Random());

            var notBefore = DateTime.UtcNow.AddDays(-1);
            var notAfter = notBefore.AddYears(validYears);
            certGen.SetSerialNumber(serial);
            certGen.SetSubjectDN(certName);
            certGen.SetIssuerDN(certName);
            certGen.SetNotBefore(notBefore);
            certGen.SetNotAfter(notAfter);
            certGen.SetPublicKey(keyPair.Public);

            // 4. Sign the Certificate
            var signatureFactory = new Asn1SignatureFactory("SHA256WITHRSA", keyPair.Private);
            var certificate = certGen.Generate(signatureFactory);

            // 5. Covert to Pkcs12, Contains Private Key and Certificate
            var store = new Pkcs12StoreBuilder().Build();
            store.SetKeyEntry(subjectName, new AsymmetricKeyEntry(keyPair.Private), new[] { new X509CertificateEntry(certificate) });

            using var stream = File.OpenWrite(path);
            store.Save(stream, password?.ToCharArray(), new SecureRandom());
        }
    }
}
