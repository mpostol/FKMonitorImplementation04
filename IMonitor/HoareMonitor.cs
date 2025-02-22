namespace MonitorImplementation.HoareMonitor
{
    public abstract class HoareMonitor
    {
        protected interface ISignal
        {
            void Send();

            void Wait();

            bool Await();

            void Dispose();
        }

        protected abstract ISignal CreateSignal();
    }
}