using System;
using System.Collections.Generic;
using System.Linq;
using NaiveAuth;

// Pros:
// - easy to write
// - simpler if implementation remains the same
// Cons:
// - duplicated code
// - SRP violated
// - OCP violated
// - complexity and amount of code scales worse

var user = new UserData("iskorotkov", "ChainOfResponsibility");
var a1 = new AuthenticatorV1(role: "editor");
Console.WriteLine($"Authenticated and authorized: {a1.Authenticate(user)}");
var a2 = new AuthenticatorV2(maxAttempts: 3, role: "editor");
Console.WriteLine($"Authenticated and authorized: {a2.Authenticate(user)}");
var a3 = new AuthenticatorV3(maxAttempts: 3, role: "editor");
Console.WriteLine($"Authenticated and authorized: {a3.Authenticate(user)}");

namespace NaiveAuth
{
    public sealed record UserData(string Username, string Password);

    public interface IAuthenticator
    {
        bool Authenticate(UserData user);
    }

    public class AuthenticatorV1 : IAuthenticator
    {
        private readonly string _role;

        public AuthenticatorV1(string role)
        {
            _role = role;
        }

        public bool Authenticate(UserData user)
        {
            return !ContainsMaliciousChars(user)
                   && AreCredentialsValid(user)
                   && CheckCredentialsInDb(user)
                   && GetUserRolesFromDb(user).Contains(_role);
        }

        private static bool AreCredentialsValid(UserData user)
        {
            return !string.IsNullOrWhiteSpace(user.Username)
                   && !string.IsNullOrWhiteSpace(user.Password);
        }

        private bool ContainsMaliciousChars(UserData user)
        {
            return user.Username.Any(IsMaliciousChar);
        }

        private bool IsMaliciousChar(char c) => @"\@<>".Contains(c);
        private bool CheckCredentialsInDb(UserData user) => true;
        private IEnumerable<string> GetUserRolesFromDb(UserData user) => new[] { "editor" };
    }

    public class AuthenticatorV2 : IAuthenticator
    {
        private readonly int _maxAttempts;
        private readonly string _role;

        public AuthenticatorV2(int maxAttempts, string role)
        {
            _maxAttempts = maxAttempts;
            _role = role;
        }

        public bool Authenticate(UserData user)
        {
            return !ContainsMaliciousChars(user)
                   && AreCredentialsValid(user)
                   && !IsBruteforceDetected(user, _maxAttempts)
                   && CheckCredentialsInDb(user)
                   && GetUserRolesFromDb(user).Contains(_role);
        }

        private bool IsBruteforceDetected(UserData user, int maxAttempts)
        {
            return GetPreviousAttemptsFromDb(user) >= maxAttempts;
        }

        private static bool AreCredentialsValid(UserData user)
        {
            return !string.IsNullOrWhiteSpace(user.Username)
                   && !string.IsNullOrWhiteSpace(user.Password);
        }

        private bool ContainsMaliciousChars(UserData user)
        {
            return user.Username.Any(IsMaliciousChar);
        }

        private bool IsMaliciousChar(char c) => @"\@<>".Contains(c);
        private int GetPreviousAttemptsFromDb(UserData user) => 3;
        private bool CheckCredentialsInDb(UserData user) => true;
        private IEnumerable<string> GetUserRolesFromDb(UserData user) => new[] { "editor" };
    }

    public class AuthenticatorV3 : IAuthenticator
    {
        private readonly string[] _cache = { "iskorotkov" };
        private readonly int _maxAttempts;
        private readonly string _role;

        public AuthenticatorV3(int maxAttempts, string role)
        {
            _maxAttempts = maxAttempts;
            _role = role;
        }

        public bool Authenticate(UserData user)
        {
            return LookupInCache(user)
                   || !ContainsMaliciousChars(user)
                   && AreCredentialsValid(user)
                   && !IsBruteforceDetected(user, _maxAttempts)
                   && CheckCredentialsInDb(user)
                   && GetUserRolesFromDb(user).Contains(_role);
        }

        private bool IsBruteforceDetected(UserData user, int maxAttempts)
        {
            return GetPreviousAttemptsFromDb(user) >= maxAttempts;
        }

        private static bool AreCredentialsValid(UserData user)
        {
            return !string.IsNullOrWhiteSpace(user.Username)
                   && !string.IsNullOrWhiteSpace(user.Password);
        }

        private bool ContainsMaliciousChars(UserData user)
        {
            return user.Username.Any(IsMaliciousChar);
        }

        private bool LookupInCache(UserData user)
        {
            return _cache.Contains(user.Username);
        }

        private bool IsMaliciousChar(char c) => @"\@<>".Contains(c);
        private int GetPreviousAttemptsFromDb(UserData user) => 3;
        private bool CheckCredentialsInDb(UserData user) => true;
        private IEnumerable<string> GetUserRolesFromDb(UserData user) => new[] { "editor" };
    }
}
