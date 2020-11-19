using System;
using Routing;

// Pros:
// - can add (remove, modify) handlers at runtime
// - SRP
// - OCP

var router = new MatchPattern<string>("/home", "Welcome to website",
    new MatchPattern<string>("/about", "Genius. Playboy. Philanthropist.",
        new MatchAll<string>("Error 404. Nothing here")));

Console.WriteLine($"/home: {router.Route("/home")}");
Console.WriteLine($"/about: {router.Route("/about")}");
Console.WriteLine($"/pricing: {router.Route("/pricing")}");

router = new MatchPattern<string>("/pricing", "Basic: $99.00, premium: $199.00", router);
Console.WriteLine($"/pricing: {router.Route("/pricing")}");

namespace Routing
{
    public abstract class Rule<T>
    {
        public Rule(T content, Rule<T>? next = null)
        {
            Content = content;
            Next = next;
        }

        public T Content { get; set; }
        public Rule<T>? Next { get; set; }

        public virtual T? Route(string request) => Next != null ? Next.Route(request) : default;
    }

    public class MatchPattern<T> : Rule<T>
    {
        public MatchPattern(string pattern, T content, Rule<T>? next = null) : base(content, next) =>
            Pattern = pattern;

        public string Pattern { get; set; }

        public override T? Route(string request)
        {
            return string.Equals(request, Pattern, StringComparison.InvariantCultureIgnoreCase)
                ? Content
                : base.Route(request);
        }
    }

    public class MatchAll<T> : Rule<T>
    {
        public MatchAll(T content, Rule<T>? next = null) : base(content, next)
        {
        }

        public override T? Route(string request) => Content;
    }
}
