using System;
using System.Collections.Generic;
using System.Linq;
using Auth;

// Read from config files/env variables/command line arguments
const bool useCache = true;
const bool blockBruteForce = true;

// Chain root
var chain = new AuthHandler();

// Add handlers
if (useCache)
{
    chain.Add(new Cache());
}

chain.Add(new Sanitize());
chain.Add(new Validate());
if (blockBruteForce)
{
    chain.Add(new BlockBruteForce(maxAttempts: 3));
}

chain.Add(new Authenticate());
chain.Add(new Authorize(role: "admin"));

// Evaluate
var user = new UserData("@iskorotkov", "ChainOfResponsibility");
Console.WriteLine($"Athenticated and authorized: {chain.Handle(user)}");

namespace Auth
{
    public sealed record UserData(string Username, string Password);

    public class AuthHandler
    {
        private AuthHandler? Next { get; set; }

        public bool Handle(UserData user)
        {
            Console.WriteLine($"> Enter {ToString()}");
            var result = Execute(user);
            Console.WriteLine($"< Exit {ToString()}");
            return result;
        }

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

        protected bool ExecuteNext(UserData user) => Next?.Handle(user) ?? true;

        protected virtual bool Execute(UserData user) => ExecuteNext(user);
    }

    public class Cache : AuthHandler
    {
        protected override bool Execute(UserData user) => IsCached(user) || ExecuteNext(user);
        private bool IsCached(UserData user) => GetCache().Any(u => u == user.Username);
        private static string[] GetCache() => new[] { "iskorotkov999" };
    }

    public class Sanitize : AuthHandler
    {
        protected override bool Execute(UserData user) => !user.Username.Any(IsDangerousSymbol) && ExecuteNext(user);
        private bool IsDangerousSymbol(char c) => @"\@<>".Contains(c);
    }

    public class Validate : AuthHandler
    {
        protected override bool Execute(UserData user) =>
            !string.IsNullOrWhiteSpace(user.Username)
            && !string.IsNullOrWhiteSpace(user.Password)
            && ExecuteNext(user);
    }

    public class BlockBruteForce : AuthHandler
    {
        private readonly int _maxAttempts;

        public BlockBruteForce(int maxAttempts) => _maxAttempts = maxAttempts;

        protected override bool Execute(UserData user) =>
            GetPreviousAttemptsFromDb(user) < _maxAttempts && ExecuteNext(user);

        private int GetPreviousAttemptsFromDb(UserData user) => 0;
    }

    public class Authenticate : AuthHandler
    {
        protected override bool Execute(UserData user) => CheckCredentialsInDb(user) && ExecuteNext(user);
        private bool CheckCredentialsInDb(UserData user) => true;
    }

    public class Authorize : AuthHandler
    {
        private readonly string _role;

        public Authorize(string role) => _role = role;
        protected override bool Execute(UserData user) => GetUserRolesFromDb(user).Contains(_role) && ExecuteNext(user);
        private IEnumerable<string> GetUserRolesFromDb(UserData user) => new[] { "editor" };
    }
}
