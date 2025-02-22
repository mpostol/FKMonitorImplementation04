using MonitorImplementation.HoareMonitor;

namespace MonitorTest
{
    [TestClass]
    public class SignalTest
    {
        [TestMethod]
        public void TestSignal()
        {
            SignalTestClass signalTestClass = new();
            // Prepare
            bool signalSent = false;
            signalTestClass.CreateTheSignal();

            Thread thread = new Thread(() =>
            {
                signalTestClass.SendTheSignal();
                signalSent = true;
            });

            // Act
            thread.Start();
            thread.Join();

            // Test
            Assert.IsTrue(signalSent);

            // Dispose
            signalTestClass.Dispose();
        }

        [TestMethod]
        public void TestWait()
        {
            // Prepare
            SignalTestClass signalTestClass2 = new();
            bool ThreadWaited = false;
            signalTestClass2.CreateTheSignal();

            Thread thread = new Thread(() =>
            {
                signalTestClass2.WaitSignal();
                ThreadWaited = true;
            });

            // Act
            thread.Start();
            Thread.Sleep(100);
            signalTestClass2.SendTheSignal();
            thread.Join();

            // Test
            Assert.IsTrue(ThreadWaited);

            // Dispose
            signalTestClass2.Dispose();
        }

        private class SignalTestClass : HoareMonitorImplementation, IDisposable
        {
            private ISignal? signal;

            internal void CreateTheSignal()
            {
                enterTheMonitor();
                try
                {
                    signal = CreateSignal();
                }
                finally
                {
                    exitTheMonitor();
                }
            }

            internal void SendTheSignal()
            {
                enterTheMonitor();
                try
                {
                    signal?.Send();
                }
                finally
                {
                    exitTheMonitor();
                }
            }

            internal void WaitSignal()
            {
                enterTheMonitor();
                try
                {
                    signal?.Wait();
                }
                finally
                {
                    exitTheMonitor();
                }
            }
        }
    }
}