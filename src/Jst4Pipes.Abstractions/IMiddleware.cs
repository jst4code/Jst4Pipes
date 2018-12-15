using System;

namespace Jst4Pipes.Abstractions
{
    public interface IMiddleware<TContext>
    {
        void Execute(TContext context, Action<TContext> next);
    }
}
