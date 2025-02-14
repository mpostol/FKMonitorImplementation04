using MonitorImplementation.HoareMonitor;

namespace MonitorTest
{
    [TestClass]
    public class BoundedBufferTest
    {
        [TestMethod]
        public void TestAddItem()
        {
            // Prepare
            BoundedBuffer buffer = new BoundedBuffer();
            int item = 0;

            // Act
            Thread threadAdd = new Thread(() =>
            {
                buffer.AddItem(2);
                item = buffer.RemoveItem();
            });

            threadAdd.Start();
            threadAdd.Join();

            // Test
            Assert.AreEqual(2, item);

            // Dispose
            buffer.Dispose();
        }

        [TestMethod]
        public void TestRemoveItem()
        {
            // Prepare
            BoundedBuffer buffer = new BoundedBuffer();
            int item = 0;

            Thread threadRemove = new Thread(() =>
            {
                item = buffer.RemoveItem();
            });

            Thread threadAdd = new Thread(() =>
            {
                buffer.AddItem(2);
            });

            // Act
            threadRemove.Start();
            threadAdd.Start();
            threadRemove.Join(); // Ensure remove finishes before testing
            threadAdd.Join();

            // Test
            Assert.AreEqual(2, item);

            // Dispose
            buffer.Dispose();
        }

        [TestMethod]
        public void TestBufferIsEmpty()
        {
            // Prepare
            BoundedBuffer buffer = new BoundedBuffer();
            const int count = 50;
            const int sleepTime = 10;
            bool isTrue = true;

            // Act
            Thread threadAdd = new Thread(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    buffer.AddItem(i);
                    Thread.Sleep(sleepTime);
                }
            });

            threadAdd.Start();
            Thread.Sleep(200);

            Thread threadRemove = new Thread(() =>
            {
                while (!buffer.isEmpty)
                {
                    buffer.RemoveItem();
                }
            });

            threadRemove.Start();
            threadAdd.Join();
            threadRemove.Join();

            // Test
            Assert.IsTrue(buffer.isEmpty);

            // Dispose
            buffer.Dispose();
        }

        private class BoundedBuffer : HoareMonitorImplementation, IDisposable
        {
            private readonly ISignal? nonempty;
            private readonly ISignal? nonfull;
            private const int N = 10;
            private readonly int[] buffer = new int[N];
            private int lastPointer = 0;
            private int count = 0;
            internal bool isfull = false;
            internal bool isEmpty = false;

            public BoundedBuffer()
            {
                nonempty = CreateSignal();
                nonfull = CreateSignal();
            }

            internal void AddItem(int x)
            {
                enterMonitorSection();
                try
                {
                    if (count == N)
                    {
                        isfull = true;
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
                    if (count == 0)
                    {
                        isEmpty = true;
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
