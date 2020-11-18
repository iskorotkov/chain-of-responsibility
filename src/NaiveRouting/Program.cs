using System;
using System.Collections.Generic;
using NaiveRouting;

// Pros:
// - can add (remove, modify) handlers at runtime
// - easy to write and use
// - SRP
// - OCP
// Cons:
// - handlers have no particular order

var router = new Router<string>
{
    Routes =
    {
        ["/home"] = "Welcome to website",
        ["/about"] = "Genius. Playboy. Philanthropist.",
        ["*"] = "Error 404. Nothing here"
    }
};

Console.WriteLine($"/home: {router.Route("/home")}");
Console.WriteLine($"/about: {router.Route("/about")}");
Console.WriteLine($"/pricing: {router.Route("/pricing")}");

router.Routes.Add("/pricing", "Basic: $99.00, premium: $199.00");
Console.WriteLine($"/pricing: {router.Route("/pricing")}");

namespace NaiveRouting
{
    public class Router<T>
    {
        public Dictionary<string, T> Routes { get; } = new();

        public T? Route(string request)
        {
            foreach (var (route, content) in Routes)
            {
                if (route == "*" || string.Equals(route, request, StringComparison.InvariantCultureIgnoreCase))
                {
                    return content;
                }
            }

            return default;
        }
    }
}
