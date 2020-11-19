using System;
using Routing;

// Pros:
// - can add (remove, modify) handlers at runtime
// - easy to use for client
// - SRP
// - OCP

var router = new Rule<string>("/home", "Welcome to website",
    new Rule<string>("/about", "Genius. Playboy. Philanthropist.",
        new Rule<string>("*", "Error 404. Nothing here")));

Console.WriteLine($"/home: {router.Route("/home")}");
Console.WriteLine($"/about: {router.Route("/about")}");
Console.WriteLine($"/pricing: {router.Route("/pricing")}");

router = new Rule<string>("/pricing", "Basic: $99.00, premium: $199.00", router);
Console.WriteLine($"/pricing: {router.Route("/pricing")}");

namespace Routing
{
    public class Rule<T>
    {
        public Rule(string pattern, T content, Rule<T>? next = null)
        {
            Next = next;
            Pattern = pattern;
            Content = content;
        }

        public Rule<T>? Next { get; set; }
        public string Pattern { get; set; }
        public T Content { get; set; }

        public T? Route(string request)
        {
            if (Pattern == "*" || string.Equals(request, Pattern, StringComparison.InvariantCultureIgnoreCase))
            {
                return Content;
            }

            return Next != null ? Next.Route(request) : default;
        }
    }
}
