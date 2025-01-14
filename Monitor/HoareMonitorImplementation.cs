using MonitorImplementation.HoareMonitor;
using System.Collections;

public class HoareMonitorImplementation : HoareMonitor
{
    public HoareMonitorImplementation()
    {

    }

    private HoareMonitorImplementation hoareMonitorImplementation;

    private class Signal : ISignal
    {
        
        Queue threadQueue = new Queue();

        private HoareMonitorImplementation hoareMonitor;

        private object dedicatedObject = new object();

        private bool checkQueue()
        {
            if(threadQueue.Count > 0) { return true; }
            else { return false; }
        }

        public Signal(HoareMonitorImplementation hoareMonitor)
        {
            this.hoareMonitor = hoareMonitor;
        }

        public void Send()
        {
            lock (dedicatedObject)
            {
                if (checkQueue())
                {
                    object ?thread = threadQueue.Dequeue();
                    Monitor.Pulse(dedicatedObject);
                }
            }

        }

        public void Wait()
        {
            lock (dedicatedObject)
            {
                threadQueue.Enqueue(Thread.CurrentThread);
                Monitor.Wait(Thread.CurrentThread);
                //threadQueue.Dequeue();
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
                    Monitor.PulseAll(dedicatedObject);
                }
            }

        }

        public void Wait()
        {
            lock (dedicatedObject)
            {
                //threadQueue.Enqueue(Thread.CurrentThread);
                Monitor.Wait(Thread.CurrentThread);
                //threadQueue.Dequeue();
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
        return new Signal(hoareMonitorImplementation);
    }

    protected override ICondition GetCondition()
    {
        return new Condition(hoareMonitorImplementation);
    }
}
