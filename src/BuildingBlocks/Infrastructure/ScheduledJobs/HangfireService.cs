using Contracts.ScheduledJobs;
using System.Linq.Expressions;
using Hangfire;

namespace Infrastructure.ScheduledJobs
{
    public class HangfireService : IScheduledJobService
    {
        public string ContinueQueueWith(string parentJobId, Expression<Action> functionCall)
        {
            return BackgroundJob.ContinueJobWith(parentJobId, functionCall);
        }

        public bool Delete(string jobId)
        {
            return BackgroundJob.Delete(jobId);
        }

        public string Enqueue(Expression<Action> functionCall)
        {
            return BackgroundJob.Enqueue(functionCall);
        }

        public string Enqueue<T>(Expression<Action<T>> funtionCall)
        {
            return BackgroundJob.Enqueue<T>(funtionCall);
        }

        public bool Requeue(string jobId)
        {
            return BackgroundJob.Requeue(jobId);
        }

        public string Schedule(Expression<Action> funtionCall, TimeSpan delay)
        {
            return BackgroundJob.Schedule(funtionCall, delay);
        }

        public string Schedule<T>(Expression<Action<T>> funtionCall, TimeSpan delay)
        {
            return BackgroundJob.Schedule<T>(funtionCall, delay);
        }

        public string Schedule(Expression<Action> funtionCall, DateTimeOffset enqueueDate)
        {
            return BackgroundJob.Schedule(funtionCall, enqueueDate);
        }

        public string Schedule<T>(Expression<Action<T>> funtionCall, DateTimeOffset enqueueDate)
        {
            return BackgroundJob.Schedule<T>(funtionCall, enqueueDate);
        }
    }
}
