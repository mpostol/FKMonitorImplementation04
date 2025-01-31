using MonitorImplementation.HoareMonitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorImplementation.HoareMonitor
{
    public class Signal : ISignal, IDisposable
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
                if (signalQueue.Count > 0)
                {
                    hoareMonitorImp.addToQueue(signalQueue.Dequeue());
                    autoResetEvent.Set();
                }
            }
        }

        public void Wait()
        {
            lock (this)
            {
                signalQueue.Enqueue(Thread.CurrentThread);
                hoareMonitorImp.enterMonitorSection();
                autoResetEvent.WaitOne();
                hoareMonitorImp.exitHoareMonitorSection();
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
}
