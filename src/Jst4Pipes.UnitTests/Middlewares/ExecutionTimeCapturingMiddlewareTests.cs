using FluentAssertions;
using FluentAssertions.Equivalency;
using Jst4Pipes.Middlewares;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Jst4Pipes.UnitTests.Middlewares
{
    public class ExecutionTimeCapturingMiddlewareTests
    {
        [Theory]
        [InlineData(1000)]
        [InlineData(5000)]
        [InlineData(20000)]
        [InlineData(20)]
        [InlineData(10)]
        public void Should_capture_execution_time_with_precision_of_20ms(int executionInMilliseconds)
        {
            TimeSpan ts = new TimeSpan();
            var middleware = new ExecutionTimeCapturingMiddleware<int>(t => ts = t);
            middleware.Execute(0, i => Task.Delay(executionInMilliseconds).Wait());

            ts.TotalMilliseconds.Should().BeInRange(executionInMilliseconds, executionInMilliseconds + 20);
        }

        public void Should_be_able_to_extens_execution_time_capture_by_inheritence(int executionInMilliseconds)
        {
            var logger = Substitute.For<ILogger>();
            var middleware = new ExecutionTimeLogginMiddleware<int>(logger);
            middleware.Execute(0, i => Task.Delay(executionInMilliseconds).Wait());

            logger.Received(1).LogTime(Arg.Is<TimeSpan>(ts => { ts.TotalMilliseconds.Should().BeInRange(executionInMilliseconds, executionInMilliseconds + 20); return true; }));
        }

    }
    public interface ILogger
    {
        void LogTime(TimeSpan ts);
    }

    public class ExecutionTimeLogginMiddleware<TEntity> : ExecutionTimeCapturingMiddleware<TEntity>
    {
        public ExecutionTimeLogginMiddleware(ILogger logger) : base(logger.LogTime)
        {
        }
    }

    public class EquivalentArgumentMatcher<T> : IArgumentMatcher, IDescribeNonMatches
    {
        private static readonly ArgumentFormatter DefaultArgumentFormatter = new ArgumentFormatter();
        private readonly object _expected;
        private readonly Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> _options;

        public EquivalentArgumentMatcher(object expected)
            : this(expected, x => x.IncludingAllDeclaredProperties())
        {
        }

        public EquivalentArgumentMatcher(object expected, Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> options)
        {
            _expected = expected;
            _options = options;
        }

        public override string ToString()
        {
            return DefaultArgumentFormatter.Format(_expected, false);
        }

        public string DescribeFor(object argument)
        {
            try
            {
                ((T)argument).ShouldBeEquivalentTo(_expected, _options);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public bool IsSatisfiedBy(object argument)
        {
            try
            {
                ((T)argument).ShouldBeEquivalentTo(_expected, _options);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
    public static class NSubstituteExtensions
    {
        public static T Equivalent<T>(this object obj)
        {
            SubstitutionContext.Current.EnqueueArgumentSpecification(new ArgumentSpecification(typeof(T), new EquivalentArgumentMatcher<T>(obj)));
            return default(T);
        }

        public static T Equivalent<T>(this T obj)
        {
            return Equivalent<T>((object)obj);
        }

        public static T Equivalent<T>(this object obj, Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> options)
        {
            SubstitutionContext.Current.EnqueueArgumentSpecification(new ArgumentSpecification(typeof(T), new EquivalentArgumentMatcher<T>(obj, options)));
            return default(T);
        }

        public static T Equivalent<T>(this T obj, Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> options)
        {
            return Equivalent((object)obj, options);
        }
    }
}
