using MonitorImplementation.HoareMonitor;
using System.Collections;

public class HoareMonitorImplementation : HoareMonitor
{
    public HoareMonitorImplementation()
    {

    }

    private object dedicatedObject = new object();

    private Queue<Thread> threadQueue = new();

    private class Signal : ISignal
    {
        private readonly HoareMonitorImplementation hoareMonitor = new();

        public Signal(HoareMonitorImplementation hoareMonitorImplementation)
        {
            hoareMonitor = hoareMonitorImplementation;
        }

        public void Send()
        {
            lock (hoareMonitor.dedicatedObject)
            {
                if (hoareMonitor.threadQueue.Count > 0)
                {
                    Thread thread = hoareMonitor.threadQueue.Dequeue();
                    Monitor.Pulse(hoareMonitor.dedicatedObject);
                }
            }

        }

        public void Wait()
        {
            lock (hoareMonitor.dedicatedObject)
            {
                hoareMonitor.threadQueue.Enqueue(Thread.CurrentThread);
                Monitor.Wait(hoareMonitor.dedicatedObject);
            }
        }

        public bool Await()
        {
            lock (hoareMonitor.dedicatedObject)
            {
                if (hoareMonitor.threadQueue.Count > 0) { return true; }
                else { return false; }
            }

        }
    }

    private class Condition : ICondition
    {
        Queue threadQueue = new Queue();

        private HoareMonitorImplementation hoareMonitor;

        private object dedicatedObject = new object();

        private bool checkQueue()
        {
            if (threadQueue.Count > 0) { return true; }
            else { return false; }
        }

        public Condition(HoareMonitorImplementation hoareMonitor)
        {
            this.hoareMonitor = hoareMonitor;
        }

        public void Send()
        {
            lock (dedicatedObject)
            {
                if (checkQueue())
                {
                    Monitor.Pulse(this);
                }
            }

        }

        public void Wait()
        {
            lock (dedicatedObject)
            {
                threadQueue.Enqueue(Thread.CurrentThread);
                Monitor.Wait(Thread.CurrentThread);
            }
        }

        public bool Await()
        {
            lock (dedicatedObject)
            {
                return checkQueue();
            }

        }
    }

    protected override ISignal CreateSignal()
    {
        return new Signal(this);
    }

    protected override ICondition GetCondition()
    {
        return new Condition(this);
    }

    public void addThreadToQueue(Thread thread)
    {
        threadQueue.Enqueue(thread);
    }
}
