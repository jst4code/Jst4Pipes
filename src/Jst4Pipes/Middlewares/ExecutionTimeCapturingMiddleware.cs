using Jst4Pipes.Abstractions;
using System;
using System.Diagnostics;

namespace Jst4Pipes.Middlewares
{
    public class ExecutionTimeCapturingMiddleware<TEntity> : IMiddleware<TEntity>
    {
        private readonly Action<TimeSpan> _onCapture;

        public ExecutionTimeCapturingMiddleware(Action<TimeSpan> onCapture)
        {
            _onCapture = onCapture;
        }
        
        public void Execute(TEntity context, Action<TEntity> next)
        {
            Stopwatch sw = Stopwatch.StartNew();
            next(context);
            sw.Stop();
            _onCapture(sw.Elapsed);
        }
    }
}
