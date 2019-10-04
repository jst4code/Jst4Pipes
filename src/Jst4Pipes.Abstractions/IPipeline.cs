using System.Threading.Tasks;

namespace Jst4Pipes.Abstractions
{
    public interface IPipeline<TContext>
    {
        IPipeline<TContext> Add(IMiddleware<TContext> filter);
        TContext Execute(TContext context);
    }
}
