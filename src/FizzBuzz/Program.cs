using System;
using FizzBuzz;

var handler = new FizzBuzzHandler
{
    Next = new FizzHandler
    {
        Next = new BuzzHandler()
    }
};
for (var request = 1; request <= 100; request++)
{
    Console.WriteLine(handler.Handle(request));
}

namespace FizzBuzz
{
    public abstract class BaseHandler
    {
        public BaseHandler? Next { get; init; }
        public virtual string Handle(int request) => Next?.Handle(request) ?? request.ToString();
    }

    public class FizzBuzzHandler : BaseHandler
    {
        public override string Handle(int request) => request % 15 == 0 ? "FizzBuzz" : base.Handle(request);
    }

    public class FizzHandler : BaseHandler
    {
        public override string Handle(int request) => request % 3 == 0 ? "Fizz" : base.Handle(request);
    }

    public class BuzzHandler : BaseHandler
    {
        public override string Handle(int request) => request % 5 == 0 ? "Buzz" : base.Handle(request);
    }
}
