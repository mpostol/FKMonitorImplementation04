using MonitorImplementation.HoareMonitor;

namespace MonitorTest
{
    [TestClass]
    public class SignalTest
    {
        [TestMethod]
        public void TestWait()
        {
            SignalTestClass signalTestClass = new();
            // Prepare
            bool isFalse = false;
            signalTestClass.CreateTheSignal();

            Thread thread = new Thread(() =>
            {
                signalTestClass.WaitSignal();
                isFalse = true;
            });

            // Act
            thread.Start();
            thread.Join();

            // Test
            Assert.IsFalse(isFalse);

            // Dispose
            signalTestClass.Dispose();
        }

        [TestMethod]
        public void TestSignal()
        {
            // Prepare
            SignalTestClass signalTestClass2 = new();
            bool isTrue = false;
            signalTestClass2.CreateTheSignal();

            Thread thread = new Thread(() =>
            {
                signalTestClass2.WaitSignal();
                isTrue = true;
            });

            // Act
            thread.Start();
            signalTestClass2.SendTheSignal();
            thread.Join();

            // Test
            Assert.IsTrue(isTrue);

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