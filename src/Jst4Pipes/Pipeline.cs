using Jst4Pipes.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jst4Pipes
{
    public class Pipeline<TContext> : IPipeline<TContext>
    {
        private List<IMiddleware<TContext>> _middlewares = new List<IMiddleware<TContext>>();

        public IPipeline<TContext> Add(IMiddleware<TContext> filter)
        {
            _middlewares.Add(filter);
            return this;
        }

        public TContext Execute(TContext context) 
            => MiddlewaresWithSequence()
                .Aggregate(
                    _defaultHandler, 
                    (next, middleware) => middleware.PartialRight(next))
                (context);

        private IEnumerable<IMiddleware<TContext>> MiddlewaresWithSequence() => _middlewares.AsEnumerable().Reverse().ToList();

        private static Func<TContext, TContext> _defaultHandler = ctx => ctx;
    }

    public static class MiddlewareExtensions
    { 
        public static Func<TContext, TContext> PartialRight<TContext>(this IMiddleware<TContext> middleware, Func<TContext, TContext> next)
            => PartialExecutionHelperMethods.PartialRight<TContext, Func<TContext, TContext>>(middleware.Execute, next);
    }
}
