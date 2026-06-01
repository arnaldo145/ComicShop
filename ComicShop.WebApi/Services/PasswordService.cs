using System;
using ComicShop.Application.Features.Users.Services;
using ComicShop.Domain.Features.Users;
using ComicShop.Infra.Helpers;
using Microsoft.AspNetCore.Identity;

namespace ComicShop.WebApi.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordHasher<User> _passwordHasher = new();

        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public PasswordVerificationStatus VerifyPassword(User user, string passwordHash, string providedPassword)
        {
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, passwordHash, providedPassword);

            if (verificationResult == PasswordVerificationResult.Success)
                return PasswordVerificationStatus.Success;

            if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
                return PasswordVerificationStatus.SuccessRehashNeeded;

            return IsLegacyPasswordMatch(passwordHash, providedPassword)
                ? PasswordVerificationStatus.SuccessRehashNeeded
                : PasswordVerificationStatus.Failed;
        }

        private static bool IsLegacyPasswordMatch(string passwordHash, string providedPassword)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                return false;

            try
            {
                var legacyPassword = EncryptionHelper.Decrypt(passwordHash);
                return string.Equals(legacyPassword, providedPassword, StringComparison.Ordinal);
            }
            catch
            {
                return false;
            }
        }
    }
}
