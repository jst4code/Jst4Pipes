using System;

namespace Jst4Pipes.Abstractions
{
    public interface IMiddleware<TContext>
    {
        TContext Execute(TContext context, Func<TContext, TContext> next);
    }
}
