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


        [Fact]
        public void PipeStringUsingMultiplePipes()
        {
            var pipeline =
                new Pipeline<string>()
                .Add(new PrefixContentMiddleware("Hello"))
                .Add(new SuffixContentMiddleware("World"));

            string result = pipeline.Execute("-content-");
            Assert.Equal("Hello-content-World", result);
        }

        [Fact]
        public void PipeStringUsingMultiplePipes_with_sequence()
        {
            var pipeline =
                new Pipeline<string>()
                .Add(new PrefixContentMiddleware("llo"))
                .Add(new PrefixContentMiddleware("He"))
                .Add(new SuffixContentMiddleware("Wo"))
                .Add(new SuffixContentMiddleware("rld"));

            string result = pipeline.Execute("-content-");
            Assert.Equal("Hello-content-World", result);
        }
    }

    public class Increment : IMiddleware<IntWraper>
    {
        public IntWraper Execute(IntWraper context, Func<IntWraper, IntWraper> next)
        {
            context.Count++;
            return next(context);
        }
    }

    public class PrefixSuffixContent : IMiddleware<StringWrapper>
    {
        private readonly string _suffix;

        public PrefixSuffixContent(string suffix) => _suffix = suffix;
        public StringWrapper Execute(StringWrapper context, Func<StringWrapper, StringWrapper> next)
        {
            context.Content += _suffix;
            next(context);
            context.Content += _suffix;

            return context;
        }
    }

    public class PrefixContentMiddleware : IMiddleware<string>
    {
        private readonly string _prefix;

        public PrefixContentMiddleware(string prefix) => _prefix = prefix;
        public string Execute(string context, Func<string, string> next)
        {
            return next($"{_prefix}{context}");
        }
    }

    public class SuffixContentMiddleware : IMiddleware<string>
    {
        private readonly string _suffix;

        public SuffixContentMiddleware(string suffix) => _suffix = suffix;
        public string Execute(string context, Func<string, string> next)
        {
            return next($"{context}{_suffix}");
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
}
