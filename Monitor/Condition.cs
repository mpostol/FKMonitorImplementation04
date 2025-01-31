using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonitorImplementation.HoareMonitor
{
    public class Condition : ICondition
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
}
