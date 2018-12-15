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

        public void Execute(TContext context)
        {
            Execute(context, 0);
        }

        private void Execute(TContext contaxt, int level)
        {
            if (level < _middlewares.Count)
            {
                _middlewares[level].Execute(contaxt, GetNextAction(level));
            }
        }

        private Action<TContext> GetNextAction(int level)
        {
            if (level < _middlewares.Count - 1)
            {
                Action<TContext, Action<TContext>> nextAction = _middlewares[level + 1].Execute;
                return nextAction.PartialRight(GetNextAction(level + 1));
            }
            return ctx => { };
        }
    }

    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Partial execution starting from Right parameter
        /// </summary>
        /// <param name="action">function of type (T1, T2) -> void</param>
        /// <param name="value2">parameter 2 from target action of type T2</param>
        /// <returns>function of type (T1 value1) -> void (with 1 less parameter)</returns>
        public static Action<T1> PartialRight<T1, T2>(
            this Action<T1, T2> action, T2 value2) =>
                (value1) => action(value1, value2);
    }
}
