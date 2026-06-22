using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Common.Extensions
{
    public static class TaskExtensions
    {
        public static Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            return task.IsCompleted // fast-path optimization
                ? task
                : task.ContinueWith(
                    completedTask => completedTask.GetAwaiter().GetResult(),
                    cancellationToken,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
        }

        public static void RunAsync(this Task task)
        {
            task.ConfigureAwait(false);
        }
    }
}