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