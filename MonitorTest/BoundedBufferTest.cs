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
    public class BoundedBufferTest
    {
        [TestMethod]
        public void TestAddItem()
        {
            // Prepare
            var buffer = new BoundedBuffer();
            buffer.CreateTheConditions();

            // Act
            buffer.AddItem(10);
            int result = buffer.RemoveItem();

            // Test
            Assert.AreEqual(10, result);

            // Dispose
            buffer.Dispose();
        }

        [TestMethod]
        public void TestRemoveFromEmptyBuffer()
        {
            // Prepare
            var buffer = new BoundedBuffer();
            buffer.CreateTheConditions();

            Task.Run(() =>
            {
                Thread.Sleep(100);
                buffer.AddItem(20);
            });

            // Act
            int result = buffer.RemoveItem();

            // Test
            Assert.AreEqual(20, result);

            // Dispose
            buffer.Dispose();
        }

        [TestMethod]
        public void TestBufferFullCondition()
        {
            // Prepare
            var buffer = new BoundedBuffer();
            buffer.CreateTheConditions();

            for (int i = 0; i < 5; i++)
            {
                buffer.AddItem(i);
            }

            Task.Run(() =>
            {
                Thread.Sleep(100);
                buffer.RemoveItem();
            });

            // Act
            buffer.AddItem(99);
            int result = buffer.RemoveItem();

            // Test
            Assert.AreEqual(1, result);

            // Dispose
            buffer.Dispose();
        }

        private class BoundedBuffer : HoareMonitorImplementation, IDisposable
        {
            private ICondition? nonempty;
            private ICondition? nonfull;

            internal void CreateTheConditions()
            {
                enterMonitorSection();
                try
                {
                    nonempty = CreateCondition();
                    nonfull = CreateCondition();
                }
                finally
                {
                    exitHoareMonitorSection();
                }
            }

            private int N = 5;
            private readonly int[] buffer = new int[5];
            private int lastPointer = 0;
            private int count = 0;

            internal void AddItem(int x)
            {
                enterMonitorSection();
                try
                {
                    while (count == N)
                    {
                        nonfull.Wait();
                    }

                    buffer[lastPointer] = x;
                    lastPointer = (lastPointer + 1) % N;
                    count++;

                    nonempty.Send();
                }
                finally
                {
                    exitHoareMonitorSection();
                }
            }

            internal int RemoveItem()
            {
                enterMonitorSection();
                try
                {
                    while (count == 0)
                    {
                        nonempty.Wait();
                    }

                    int x = buffer[(lastPointer - count + N) % N];
                    count--;

                    nonfull.Send();
                    return x;
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
