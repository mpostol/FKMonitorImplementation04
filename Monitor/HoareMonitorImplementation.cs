namespace MonitorImplementation.HoareMonitor
{
    public abstract class HoareMonitorImplementation : HoareMonitor, IDisposable
    {
        private bool disposedValue = false;

        private int urgentCount = 0;
        private readonly Semaphore mutex = new Semaphore(1, 1);
        private readonly Semaphore urgent = new Semaphore(0, int.MaxValue);

        protected class Signal : ISignal, IDisposable
        {
            private bool _disposed = false;
            private int condCount = 0;
            private readonly Semaphore condsem = new Semaphore(0, int.MaxValue);

            private readonly HoareMonitorImplementation hoareMonitorImp;

            public Signal(HoareMonitorImplementation monitor)
            {
                hoareMonitorImp = monitor;
            }

            public void Send()
            {
                hoareMonitorImp.urgentCount++;
                if (condCount > 0)
                {
                    condsem.Release();
                    hoareMonitorImp.urgent.WaitOne();
                }
                hoareMonitorImp.urgentCount--;
            }

            public void Wait()
            {
                condCount++;
                hoareMonitorImp.exitTheMonitor();
                condsem.WaitOne();
                condCount--;
            }

            public bool Await()
            {
                return condCount > 0;
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        condsem?.Dispose();
                    }
                    _disposed = true;
                }
            }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }

        protected internal void enterTheMonitor()
        {
            mutex.WaitOne();
        }

        protected internal void exitTheMonitor()
        {
            if (urgentCount > 0)
            {
                urgent.Release();
            }
            else
            {
                mutex.Release();
            }
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
                    mutex?.Dispose();
                    urgent?.Dispose();
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