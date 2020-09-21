using System;
using System.IO;
using System.Linq;
using Microsoft.IdentityModel.Tokens;

namespace Isotope.Code.Utils
{
    /// <summary>
    /// Helper class for working with JWT authorization.
    /// </summary>
    public static class JwtHelper
    {
        private static SymmetricSecurityKey _key;
        private static readonly object _lock = new object();

        public const string Issuer = "isotope-issuer";
        public const string Audience = "isotope-audience";
        
        /// <summary>
        /// Returns the security key.
        /// </summary>
        public static SymmetricSecurityKey GetKey()
        {
            if(_key == null)
                lock(_lock)
                    if(_key == null)
                        _key = new SymmetricSecurityKey(GetOrCreateSecret());

            return _key;
        }

        /// <summary>
        /// Returns signing credentials for the specified key.
        /// </summary>
        public static SigningCredentials GetSigningCredentials()
        {
            return new SigningCredentials(GetKey(), SecurityAlgorithms.HmacSha256);
        }
        
        /// <summary>
        /// Returns the secret value or creates a new one if none exists.
        /// </summary>
        private static byte[] GetOrCreateSecret()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "Isotope.key");
            if (File.Exists(path))
                return File.ReadAllBytes(path);

            var bytes = Enumerable.Range(1, 16).Select(x => Guid.NewGuid().ToByteArray()).SelectMany(x => x).ToArray();
            File.WriteAllBytes(path, bytes);
            return bytes;
        }
    }
}