namespace MonitorImplementation.HoareMonitor
{
    public abstract class HoareMonitorImplementation : HoareMonitor, IDisposable
    {
        private bool disposedValue;

        protected class Signal : ISignal, IDisposable
        {
            private bool _disposed = false;
            private int semaphoreCount = 0;

            private HoareMonitorImplementation hoareMonitorImp;

            private AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            public Signal(HoareMonitorImplementation monitor)
            {
                hoareMonitorImp = monitor;
            }

            public void Send()
            {
                autoResetEvent.Set();
            }

            public void Wait()
            {
                hoareMonitorImp.exitHoareMonitorSection();
                semaphoreCount++;
                autoResetEvent.WaitOne();
                semaphoreCount--;
                hoareMonitorImp.enterMonitorSection();
            }


            public bool Await()
            {
                if (semaphoreCount > 0)
                {
                    return true;
                }
                else
                {
                    return false;
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
                }

                _disposed = true;
            }
        }

        private readonly Semaphore enterexitSemaphore = new Semaphore(1, 1);
        protected internal void enterMonitorSection()
        {
            enterexitSemaphore.WaitOne();
        }

        protected internal void exitHoareMonitorSection()
        {
            enterexitSemaphore.Release();
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
                    enterexitSemaphore.Dispose();
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