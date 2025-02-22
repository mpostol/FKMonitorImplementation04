using MonitorImplementation.HoareMonitor;

namespace MonitorTest
{
    [TestClass]
    public class SignalTest
    {
        [TestMethod]
        public void TestSignal()
        {
            // Prepare
            bool signalSent = false;
            using (SignalTestClass signalTestClass = new())
            {
                //Act
                signalTestClass.CreateTheSignal();

                Thread thread = new Thread(() =>
                {
                    signalTestClass.SendTheSignal();
                    signalSent = true;
                });

                thread.Start();
                thread.Join();
            }

            // Test
            Assert.IsTrue(signalSent);
        }

        [TestMethod]
        public void TestWait()
        {
            // Prepare
            bool ThreadWaited = false;
            using (SignalTestClass signalTestClass2 = new())
            {
                signalTestClass2.CreateTheSignal();

                // Act
                Thread thread = new Thread(() =>
                {
                    signalTestClass2.WaitSignal();
                    ThreadWaited = true;
                });

                thread.Start();
                Thread.Sleep(100);
                signalTestClass2.SendTheSignal();
                thread.Join();
            }

            // Test
            Assert.IsTrue(ThreadWaited);
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