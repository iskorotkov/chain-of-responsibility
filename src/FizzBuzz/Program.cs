using System;
using FizzBuzz;

while (true)
{
    var input = Console.ReadLine();
    if (!int.TryParse(input, out var request))
    {
        Console.WriteLine("Введите число");
        continue;
    }

    var chain = new FizzHandler
    {
        Next = new BuzzHandler()
    };

    Console.WriteLine(chain.Handle(request));
}

namespace FizzBuzz
{
    public abstract class BaseHandler
    {
        public BaseHandler? Next { get; init; }

        public string Handle(int request)
        {
            var result = CanHandle(request) ? GetResponse(request) : "";
            var nextResult = Next?.GetResponse(request) ?? "";
            return result + nextResult;
        }

        protected abstract bool CanHandle(int request);
        protected abstract string GetResponse(int request);
    }

    public class FizzHandler : BaseHandler
    {
        protected override bool CanHandle(int request) => request % 3 == 0;
        protected override string GetResponse(int request) => "Fizz";
    }

    public class BuzzHandler : BaseHandler
    {
        protected override bool CanHandle(int request) => request % 5 == 0;
        protected override string GetResponse(int request) => "Buzz";
    }
}
