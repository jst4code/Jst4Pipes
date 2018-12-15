using Jst4Pipes.Abstractions;
using System;
using Xunit;

namespace Jst4Pipes.UnitTests
{
    public class PipeTests
    {
        [Fact]
        public void PipeInt()
        {
            var pipeline = new Pipeline<IntWraper>()
                .Add(new Increment())
                .Add(new Increment());
            IntWraper initialValue = 0;
            pipeline.Execute(initialValue);
            Assert.Equal(2, initialValue.Count);
        }

        [Fact]
        public void PipeString()
        {
            var pipeline = 
                new Pipeline<StringWrapper>()
                .Add(new PrefixSuffixContent("Hello"))
                .Add(new PrefixSuffixContent("World"));

            StringWrapper initialValue = "";
            pipeline.Execute(initialValue);
            Assert.Equal("HelloWorldWorldHello", initialValue.Content);
        }
    }

    public class Increment : IMiddleware<IntWraper>
    {
        public void Execute(IntWraper context, Action<IntWraper> next)
        {
            context.Count++;
            next(context);
        }
    }

    public class PrefixSuffixContent : IMiddleware<StringWrapper>
    {
        private readonly string _suffix;

        public PrefixSuffixContent(string suffix) => _suffix = suffix;
        public void Execute(StringWrapper context, Action<StringWrapper> next)
        {
            context.Content += _suffix;
            next(context);
            context.Content += _suffix;
        }
    }

    public class IntWraper
    {
        public IntWraper(int count) => Count = count;
        public int Count { get; set; }
        public string Content { get; set; }
        public static implicit operator IntWraper(int value) => new IntWraper(value);
    }

    public class StringWrapper
    {
        public StringWrapper(string content) => Content = content;

        public string Content { get; set; }

        public static implicit operator StringWrapper(string value) => new StringWrapper(value);
    }

    //public class LoggingFilter<TContent> : Filter<TContent>
    //{
    //    protected override async Task Execute(TContent context, Func<TContent, Task> next)
    //    {
    //        Console.WriteLine("Before Request");
    //        await next(context);
    //        Console.WriteLine("After Request");
    //    }
    //}

    //public class TimingFilter<TContent> : Filter<TContent>
    //{
    //    protected override async Task Execute(TContent context, Func<TContent, Task> next)
    //    {
    //        var sw = new Stopwatch();
    //        sw.Start();
    //        await next(context);
    //        sw.Stop();

    //        Console.WriteLine($"Completed request in {sw.ElapsedMilliseconds}ms");
    //    }
    //}

}
