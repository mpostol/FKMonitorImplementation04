using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MonitorImplementation.HoareMonitor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MonitorTest
{
    [TestClass]
    public class SignalTest
    {
        private SignalTestClass signalTestClass = new();
        private SignalTestClass signalTestClass2 = new();

        [TestMethod]
        public void TestWait()
        {
            // Prepare
            bool waited = false;
            signalTestClass.CreateTheSignal();

            Thread thread = new Thread(() =>
            {
                signalTestClass.WaitSignal();
                waited = true;
            });

            // Act
            thread.Start();
            Thread.Sleep(100);

            // Test
            Assert.IsFalse(waited);

            // Dispose
            signalTestClass.Dispose();
        }

        [TestMethod]
        public void TestSignal()
        {
            // Prepare
            bool waited = false;
            signalTestClass2.CreateTheSignal();

            Thread thread = new Thread(() =>
            {
                signalTestClass2.WaitSignal();
                waited = true;
            });

            // Act
            thread.Start();
            Thread.Sleep(100);
            signalTestClass2.SendTheSignal();
            thread.Join();

            // Test
            Assert.IsTrue(waited);

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
                    signal.Send();
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
                    if (signal != null)
                    {
                        signal.Wait();
                    }
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