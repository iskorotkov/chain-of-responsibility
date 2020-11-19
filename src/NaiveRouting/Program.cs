using System;
using System.Collections.Generic;
using NaiveRouting;

// Pros:
// - can add (remove, modify) handlers at runtime
// - easy to write and use
// - SRP
// - OCP

var router = new Router<string>
{
    Routes =
    {
        new Route<string>("/home", "Welcome to website"),
        new Route<string>("/about", "Genius. Playboy. Philanthropist."),
        new Route<string>("*", "Error 404. Nothing here")
    }
};
Console.WriteLine($"/home: {router.Route("/home")}");
Console.WriteLine($"/about: {router.Route("/about")}");
Console.WriteLine($"/pricing: {router.Route("/pricing")}");
router.Routes.Insert(0, new Route<string>("/pricing", "Basic: $99.00, premium: $199.00"));
Console.WriteLine($"/pricing: {router.Route("/pricing")}");

namespace NaiveRouting
{
    public sealed record Route<T>(string Pattern, T Content);

    public class Router<T>
    {
        public List<Route<T>> Routes { get; } = new();

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
