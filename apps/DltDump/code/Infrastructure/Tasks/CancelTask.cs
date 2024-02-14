namespace RJCP.App.DltDump.Infrastructure.Tasks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Resources;

    public sealed class CancelTask
    {
        private readonly ManualResetEventSlim m_Started = new(false);
        private readonly CancellationTokenSource m_TokenSource = new();
        private readonly Task m_Task;
        private readonly object m_CancelLock = new();

        public CancelTask(Action<CancellationToken> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            CancellationToken token = m_TokenSource.Token;
            m_Task = new Task(() => {
                m_Started.Set();
                action(token);
            }, token);
            m_Task.ConfigureAwait(false);
        }

        public Task Run()
        {
            if (m_Started.IsSet) throw new InvalidOperationException(AppResources.InfraTaskAlreadyStarted);

            lock (m_CancelLock) {
                // Avoid
                // * InvalidOperationException when starting a Task that is cancelled.
                // * TaskCanceledException because it was already cancelled.
                if (m_TokenSource.IsCancellationRequested) return Task.CompletedTask;
                m_Task.Start();
                m_Started.Wait();
            }

            return m_Task;
        }

        public Task Cancel()
        {
            lock (m_CancelLock) {
                m_TokenSource.Cancel();
            }
            if (!m_Started.IsSet) return Task.CompletedTask;
            return m_Task;
        }
    }
}
