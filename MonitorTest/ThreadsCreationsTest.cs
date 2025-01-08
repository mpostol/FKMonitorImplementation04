using MonitorImplementation.HoareMonitor;

namespace MonitorImplementation.HoareMonitorTest
{
    [TestClass]
    public class ThreadsCreationsTest
    {
        [TestMethod]
        public void StartThreadsTestMethod()
        {
            int counter = 0;
            Thread[] _Threads = new Thread[2];
            for (int i = 0; i < _Threads.Length; i++)
                _Threads[i] = ThreadsCreations.StartThread(x => Interlocked.Add(ref counter, 1));
            foreach (Thread _thread in _Threads)
                _thread.Join();
        }

        [TestMethod]
        public void tartThreadsUsingThreadPoolTestMethod()
        {
            int counter = 0;
            for (int i = 0; i < 2; i++)
                ThreadsCreations.StartThreadsUsingThreadPool(x => Interlocked.Add(ref counter, 1));
            Thread.Sleep(100); //race condition possible
            Assert.AreEqual<int>(2, counter);
        }

        [TestMethod]
        public void StartThreadsUsingTaskTestMethod()
        {
            int counter = 0;
            List<Task> _tasksInProgress = new List<Task>();
            for (int i = 0; i < 2; i++)
                _tasksInProgress.Add(ThreadsCreations.StartThreadsUsingTask(() => Interlocked.Add(ref counter, 1)));
            Task.WaitAll(_tasksInProgress.ToArray());
            Assert.AreEqual<int>(2, counter);
        }
    }
}