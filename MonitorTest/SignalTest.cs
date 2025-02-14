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
                enterMonitorSection();
                try
                {
                    signal = CreateSignal();
                }
                finally
                {
                    exitHoareMonitorSection();
                }
            }

            internal void SendTheSignal()
            {
                enterMonitorSection();
                try
                {
                    signal?.Send();
                }
                finally
                {
                    exitHoareMonitorSection();
                }
            }

            internal void WaitSignal()
            {
                enterMonitorSection();
                try
                {
                    signal?.Wait();
                }
                finally
                {
                    exitHoareMonitorSection();
                }
            }

            public new void Dispose()
            {
                base.Dispose();
            }
        }
    }
}