using System.Linq.Expressions;

namespace Contracts.ScheduledJobs
{
    public interface IScheduledJobService
    {
        string Enqueue(Expression<Action> functionCall);
        string Enqueue<T>(Expression<Action<T>> funtionCall);
        string Schedule(Expression<Action> funtionCall, TimeSpan delay);
        string Schedule<T>(Expression<Action<T>> funtionCall, TimeSpan delay);
        string Schedule(Expression<Action> funtionCall, DateTimeOffset enqueueDate);
        string Schedule<T>(Expression<Action<T>> funtionCall, DateTimeOffset enqueueDate);
        string ContinueQueueWith(string parentJobId, Expression<Action> functionCall);
        bool Delete(string jobId);
        bool Requeue(string jobId);
    }
}
