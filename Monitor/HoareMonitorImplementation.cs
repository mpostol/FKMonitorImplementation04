using MonitorImplementation.HoareMonitor;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MonitorImplementation.HoareMonitor
{
    public abstract class HoareMonitorImplementation : HoareMonitor, IDisposable
    {
        protected bool isEntered = false;
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

        protected class Condition : ICondition
        {
            private HoareMonitorImplementation hoareMonitorImp;
            private Queue<Thread> conditionQueue = new();

            public Condition(HoareMonitorImplementation monitor)
            {
                hoareMonitorImp = monitor;
            }

            public void Send()
            {
                lock (this)
                {
                    if (conditionQueue.Count > 0)
                    {
                        Thread signaledThread = conditionQueue.Dequeue();
                        hoareMonitorImp.addToQueue(signaledThread);
                        Monitor.Pulse(this);
                    }
                }
            }

            public void Wait()
            {
                lock (this)
                {
                    conditionQueue.Enqueue(Thread.CurrentThread);
                    hoareMonitorImp.exitHoareMonitorSection();
                    Monitor.Wait(this);
                    hoareMonitorImp.enterMonitorSection();
                }
            }

            public bool Await()
            {
                lock (this)
                {
                    return conditionQueue.Count > 0;
                }
            }

            public void Dispose()
            {
                lock (this)
                {
                    conditionQueue.Clear();
                }
            }
        }

        protected internal void enterMonitorSection()
        {
            Monitor.Enter(this);
            isEntered = true;
        }

        protected internal void exitHoareMonitorSection()
        {
            if (isEntered)
            {
                isEntered = false;
                Monitor.Exit(this);
                Monitor.Pulse(this);
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

        protected override ICondition CreateCondition()
        {
            return new Condition(this);
        }

        public void ExecutMethod()
        {
            Console.WriteLine("Executing a method in Critical Section.");
            //TO DO: implement AOP
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