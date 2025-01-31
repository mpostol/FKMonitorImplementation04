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
    public class HoareMonitorTest
    {
        private TestMonitor monitor = new();

        [TestMethod]
        public void TestWait()
        {
            // Prepare
            bool waited = false;
            
            Thread thread = new Thread(() =>
            {
                monitor.CriticalSection(() =>
                {
                    monitor.TestSignal.Wait();
                    waited = true;
                });
            });

            // Act
            thread.Start();
            Thread.Sleep(100);

            // Test
            Assert.IsFalse(waited);

            // Dispose
            monitor.Dispose();
        }

        [TestMethod]
        public void TestSignal()
        {
            // Prepare
            bool waited = false;
            Thread thread = new Thread(() =>
            {
                monitor.CriticalSection(() =>
                {
                    monitor.TestSignal.Wait();
                    waited = true;
                });
            });

            // Act
            thread.Start();
            Thread.Sleep(100);
            monitor.CriticalSection(() =>
            {
                monitor.TestSignal.Send();
            });
            thread.Join();

            // Test
            Assert.IsTrue(waited);

            // Dispose
            monitor.Dispose();
        }

        // Test according to C# dispose test example
        [TestMethod]
        public void TestDispose()
        {
            // Prepare
            var monitor = new TestMonitor();

            // Act
            monitor.Dispose();

            // Test
            Assert.ThrowsException<ObjectDisposedException>(() => monitor.TestSignal.Wait());

            // Dispose
            // No dispose step here, already disposed
        }

        private class TestMonitor : HoareMonitorImplementation, IDisposable
        {
            protected internal bool IsConsistent = true;
            protected internal ISignal TestSignal { get; set; }

            protected internal TestMonitor()
            {
                TestSignal = CreateSignal();
            }

            protected internal void CriticalSection(Action action)
            {
                enterMonitorSection();
                try
                {
                    action();
                }
                finally
                {
                    exitHoareMonitorSection();
                }
            }
            public new void Dispose()
            {
                base.Dispose();
                IsConsistent = false;
            }
        }
    }
}