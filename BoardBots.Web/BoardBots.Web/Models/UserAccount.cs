using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BoardBots.Web.Models
{
    public class UserAccount
    {
        public string Id { get; private set; }
        public string Username { get; set; }
        public byte[] HashedPassword { get; private set; }

        public void SetPassword(string password)
        {
            if (string.IsNullOrEmpty(Username))
                throw new InvalidOperationException("Username must be set first");
            HashedPassword = HashPassword(Username, password);
        }

        public bool CheckPassword(string password)
        {
            var attempt = HashPassword(Username, password);
            return attempt.SequenceEqual(HashedPassword);
        }

        static byte[] HashPassword(string username, string password)
        {
            var crypt = SHA256.Create();
            string salted = username + password;
            var hashed = crypt.ComputeHash(Encoding.Unicode.GetBytes(salted));
            return hashed;
        }

        public bool HasPasswordSet()
        {
            return HashedPassword != null;
        }
    }

    public class OAuthAssociation
    {
        public string Id { get; private set; }
        public string ProviderName { get; set; }
        public string ProviderUserId { get; set; }
        public string UserAccountId { get; set; }
    }
}