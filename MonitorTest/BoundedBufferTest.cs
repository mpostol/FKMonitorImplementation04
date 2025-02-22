using MonitorImplementation.HoareMonitor;

namespace MonitorTest
{
    [TestClass]
    public class BoundedBufferTest
    {
        [TestMethod]
        public void TestTransferItem()
        {
            // Prepare
            int item = 0;
            using (BoundedBuffer<int> buffer = new BoundedBuffer<int>())
            {
                // Act
                buffer.AddItem(2);
                item = buffer.RemoveItem();
            }

            // Test
            Assert.AreEqual(2, item);
        }

        [TestMethod]
        public void TestRemoveItem()
        {
            // Prepare
            int item = 0;
            using (BoundedBuffer<int> buffer = new BoundedBuffer<int>())
            {
                // Act
                Thread threadRemove = new Thread(() =>
                {
                    item = buffer.RemoveItem();
                });

                Thread threadAdd = new Thread(() =>
                {
                    buffer.AddItem(2);
                });

                threadRemove.Start();
                threadAdd.Start();
                threadRemove.Join();
                threadAdd.Join();
            }

            // Test
            Assert.AreEqual(2, item);
        }

        [TestMethod]
        public void TestBufferIsEmpty()
        {
            // Prepare
            const int count = 50;
            const int sleepTime = 10;
            bool isEmpty = false;
            using (BoundedBuffer<int> buffer = new BoundedBuffer<int>())
            {
                // Act
                Thread threadAdd = new Thread(() =>
                {
                    for (int i = 0; i < count; i++)
                    {
                        buffer.AddItem(i);
                        Thread.Sleep(sleepTime);
                    }
                });

                Thread threadRemove = new Thread(() =>
                {
                    for (int i = 0; i < count; i++)
                    {
                        buffer.RemoveItem();
                    }
                });

                threadAdd.Start();
                threadRemove.Start();
                threadAdd.Join();
                threadRemove.Join();

                isEmpty = buffer.isEmpty;
            }

            // Test
            Assert.IsTrue(isEmpty);
        }

        [TestMethod]
        public void TestBufferIsFull()
        {
            // Prepare
            const int count = 50;
            const int removeSleepTime = 10;
            bool isFull = false;
            using (BoundedBuffer<int> buffer = new BoundedBuffer<int>())
            {
                // Act
                Thread threadAdd = new Thread(() =>
                {
                    for (int i = 0; i < count; i++)
                    {
                        buffer.AddItem(i);
                        if (buffer.isFull)
                        {
                            isFull = true;
                        }
                    }
                });

                Thread threadRemove = new Thread(() =>
                {
                    for (int i = 0; i < count; i++)
                    {
                        buffer.RemoveItem();
                        Thread.Sleep(removeSleepTime);
                    }
                });

                threadAdd.Start();
                threadRemove.Start();
                threadAdd.Join();
                threadRemove.Join();
            }

            // Test
            Assert.IsTrue(isFull);
        }

        private class BoundedBuffer<T> : HoareMonitorImplementation
        {
            private readonly ISignal? nonempty;
            private readonly ISignal? nonfull;
            private const int N = 10;
            private readonly T[] buffer = new T[N];
            private int lastPointer = 0;
            private int count = 0;
            internal bool isFull = false;
            internal bool isEmpty = false;

            public BoundedBuffer()
            {
                nonempty = CreateSignal();
                nonfull = CreateSignal();
            }

            internal void AddItem(T x)
            {
                enterTheMonitor();
                try
                {
                    while (count == N)
                    {
                        isFull = true;
                        nonfull.Wait();
                    }

                    buffer[lastPointer] = x;
                    lastPointer = (lastPointer + 1) % N;
                    count++;

                    nonempty.Send();
                }
                finally
                {
                    exitTheMonitor();
                }
            }

            internal T RemoveItem()
            {
                enterTheMonitor();
                try
                {
                    while (count == 0)
                    {
                        isEmpty = true;
                        nonempty.Wait();
                    }

                    int index = (lastPointer - count + N) % N;
                    T intIndex = buffer[index];
                    count--;

                    nonfull.Send();
                    return intIndex;
                }
                finally
                {
                    exitTheMonitor();
                }
            }
        }
    }
}
