using System;
using System.Collections.Generic;
using System.Linq;
using Auth;

var user = new UserData("iskorotkov", "ChainOfResponsibility");
var a1 = new Authenticator(useCache: false, blockBruteForce: false);
Console.WriteLine($"Authenticated and authorized: {a1.Authenticate(user)}");
var a2 = new Authenticator(useCache: false, blockBruteForce: true);
Console.WriteLine($"Authenticated and authorized: {a2.Authenticate(user)}");
var a3 = new Authenticator(useCache: true, blockBruteForce: true);
Console.WriteLine($"Authenticated and authorized: {a3.Authenticate(user)}");

namespace Auth
{
    public sealed record UserData(string Username, string Password);

    public class Authenticator
    {
        private readonly AuthHandler _handler;

        public Authenticator(bool useCache, bool blockBruteForce)
        {
            // Chain root
            _handler = new AuthHandler();

            // Add handlers
            if (useCache)
            {
                _handler.Add(new Cache());
            }

            _handler.Add(new Sanitize());
            _handler.Add(new Validate());
            if (blockBruteForce)
            {
                _handler.Add(new BlockBruteForce(maxAttempts: 3));
            }

            _handler.Add(new Authenticate());
            _handler.Add(new Authorize(role: "editor"));
        }

        public bool Authenticate(UserData user) => _handler.Execute(user);
    }

    public class AuthHandler
    {
        private AuthHandler? Next { get; set; }

        public virtual bool Execute(UserData user) => Next?.Execute(user) ?? true;

        public void Add(AuthHandler handler)
        {
            if (Next != null)
            {
                Next.Add(handler);
            }
            else
            {
                Next = handler;
            }
        }
    }

    public class Cache : AuthHandler
    {
        public override bool Execute(UserData user)
        {
            Console.WriteLine(ToString());
            return IsCached(user) || base.Execute(user);
        }

        private bool IsCached(UserData user) => GetCache().Any(u => u == user.Username);
        private static string[] GetCache() => new[] { "iskorotkov" };
    }

    public class Sanitize : AuthHandler
    {
        public override bool Execute(UserData user)
        {
            Console.WriteLine(ToString());
            return !user.Username.Any(IsDangerousSymbol) && base.Execute(user);
        }

        private bool IsDangerousSymbol(char c) => @"\@<>".Contains(c);
    }

    public class Validate : AuthHandler
    {
        public override bool Execute(UserData user)
        {
            Console.WriteLine(ToString());
            return !string.IsNullOrWhiteSpace(user.Username)
                   && !string.IsNullOrWhiteSpace(user.Password)
                   && base.Execute(user);
        }
    }

    public class BlockBruteForce : AuthHandler
    {
        private readonly int _maxAttempts;

        public BlockBruteForce(int maxAttempts) => _maxAttempts = maxAttempts;

        public override bool Execute(UserData user)
        {
            Console.WriteLine(ToString());
            return GetPreviousAttemptsFromDb(user) < _maxAttempts && base.Execute(user);
        }

        private int GetPreviousAttemptsFromDb(UserData user) => 3;
    }

    public class Authenticate : AuthHandler
    {
        public override bool Execute(UserData user)
        {
            Console.WriteLine(ToString());
            return CheckCredentialsInDb(user) && base.Execute(user);
        }

        private bool CheckCredentialsInDb(UserData user) => true;
    }

    public class Authorize : AuthHandler
    {
        private readonly string _role;

        public Authorize(string role) => _role = role;

        public override bool Execute(UserData user)
        {
            Console.WriteLine(ToString());
            return GetUserRolesFromDb(user).Contains(_role) && base.Execute(user);
        }

        private IEnumerable<string> GetUserRolesFromDb(UserData user) => new[] { "editor" };
    }
}
