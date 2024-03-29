﻿namespace RJCP.App.DltDump.Infrastructure.IO
{
    using System;
    using System.Threading;

    public sealed class OneShotTimer : IDisposable
    {
        private readonly int m_DueTime;
        private readonly object m_DisposeLock = new();
        private Timer m_Timer;

        public OneShotTimer(int dueTime)
        {
            m_DueTime = dueTime;
        }

        public event EventHandler<TimerEventArgs> TimerEvent;

        private void OnTimerEvent(object sender, TimerEventArgs args)
        {
            EventHandler<TimerEventArgs> handler = TimerEvent;
            if (handler is not null) handler(sender, args);
        }

        public bool Fired { get; private set; }

        public void Start()
        {
            ThrowHelper.ThrowIfDisposed(m_IsDisposed, this);
            if (m_Timer is not null)
                throw new InvalidOperationException("Timer already initialized");

            m_Timer = new Timer(TimerExpired, this, m_DueTime, Timeout.Infinite);
        }

        public void Reset()
        {
            ThrowHelper.ThrowIfDisposed(m_IsDisposed, this);
            if (m_Timer is null) Start();

            m_Timer.Change(m_DueTime, Timeout.Infinite);
        }

        private void TimerExpired(object sender)
        {
            lock (m_DisposeLock) {
                if (m_IsDisposed) return;
                Fired = true;
                OnTimerEvent(sender, new TimerEventArgs());
            }
        }

        private bool m_IsDisposed;

        public void Dispose()
        {
            if (m_Timer is not null) {
                lock (m_DisposeLock) {
                    m_Timer.Dispose();
                    m_IsDisposed = true;
                }
            }
        }
    }
}
