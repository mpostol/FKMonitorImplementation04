using MonitorImplementation.HoareMonitor;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MonitorImplementation.HoareMonitor
{
    public abstract class HoareMonitorImplementation : HoareMonitor, IDisposable
    {
        private Queue<Thread> monitorQueue = new();
        private bool disposedValue;

        protected class Signal : ISignal, IDisposable
        {
            private bool _disposed = false;
            private Queue<Thread> signalQueue = new();

            private HoareMonitorImplementation hoareMonitorImp;

            private AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            public Signal(HoareMonitorImplementation monitor)
            {
                hoareMonitorImp = monitor;
            }

            public void Send()
            {
                lock (this)
                {
                    autoResetEvent.Set();
                }
            }

            public void Wait()
            {
                lock (this)
                {
                    hoareMonitorImp.exitHoareMonitorSection();
                    autoResetEvent.WaitOne();
                    hoareMonitorImp.enterMonitorSection();
                }
            }


            public bool Await()
            {
                lock (this)
                {
                    if (signalQueue.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (_disposed)
                {
                    return;
                }

                if (disposing)
                {
                    autoResetEvent.Dispose();
                    signalQueue.Clear();
                }

                _disposed = true;
            }
        }

        protected internal void enterMonitorSection()
        {
            Monitor.Enter(this);
        }

        protected internal void exitHoareMonitorSection()
        {
            lock (this)
            {
                Monitor.Pulse(this);
                Monitor.Exit(this);
            }
        }


        protected internal void addToQueue(Thread thread)
        {
            monitorQueue.Enqueue(thread);
        }

        protected override ISignal CreateSignal()
        {
            return new Signal(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    monitorQueue.Clear();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}