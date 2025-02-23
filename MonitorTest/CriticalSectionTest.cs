using MonitorImplementation.HoareMonitor;

namespace MonitorTest
{
    [TestClass]
    public class CriticalSectionTest
    {
        [TestMethod]
        public void TestProtectedMethod() //UNDONE exception test is expected
        {
            // Prepare
            using (CriticalSectionTestClass criticalSectionTestClass = new CriticalSectionTestClass())
            {
                // Act
                Thread threadProtected = new Thread(() =>
                {
                    criticalSectionTestClass.ThreadsMethod(criticalSectionTestClass.HoareMonitorMethod);
                });

                threadProtected.Start();
                threadProtected.Join();

                // Test
                Assert.IsTrue(criticalSectionTestClass.checkValidity);
            }
        }

        [TestMethod]
        public void TestNotProtectedMethod()
        {
            // Prepare
            using (CriticalSectionTestClass criticalSectionTestClass = new CriticalSectionTestClass())
            {
                // Act
                Thread threadProtected = new Thread(() =>
                {
                    criticalSectionTestClass.ThreadsMethod(criticalSectionTestClass.NonMonitorMethod);
                });

                threadProtected.Start();
                threadProtected.Join();

                // Test
                Assert.IsFalse(criticalSectionTestClass.checkValidity);
            }
        }

        private class CriticalSectionTestClass : HoareMonitorImplementation
        {
            internal void HoareMonitorMethod()
            {
                enterTheMonitor();
                try
                {
                    TestMethod();
                }
                finally
                {
                    exitTheMonitor();
                }
            }

            internal void NonMonitorMethod()
            {
                TestMethod();
            }

            public void ThreadsMethod(ThreadStart start) //HACK Why public (internal declaration) - monitor should be used in the multithread environment.
            {
                if (start == null)
                    throw new ArgumentNullException(nameof(start));
                Thread[] threadsArray = new Thread[2];
                for (int i = 0; i < threadsArray.Length; i++)
                    threadsArray[i] = new Thread(start);
                foreach (Thread _thread in threadsArray)
                    _thread.Start();
                foreach (Thread _thread in threadsArray)
                    _thread.Join();
            }

            private int intA = 0;
            private int intB = 0;
            private int counter = 0; //HACK Why not random?

            internal void TestMethod()  //HACK Why not private
            {
                for (int i = 0; i < 100000; i++)
                {
                    counter++;
                    intA = counter;
                    intB = counter;
                    if (intA - intB != 0)
                    {
                        checkValidity = false;
                    }
                }
            }

            public bool checkValidity = true;
        }
    }
}