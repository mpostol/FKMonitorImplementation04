using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorImplementation.HoareMonitor
{
    public interface ISignal
    {
        /// <summary>
        /// A thread that changes the state of the monitor in a way that might allow a waiting thread to proceed will signal the <see cref="ISignal"/> variable and, 
        /// as a result, awake up one of the waiting threads.
        /// </summary>
        /// <remarks>
        /// The send signal operation needs some scheduling decisions. After sending a signal the thread should immediately wake up the waiting one, if any, 
        /// and give up the monitor to it. It means that the monitor is transferred from a thread issuing a signal to the signaled one. 
        /// The suspended process will afterward regain the processor. This kind of scheduling treats the waiting process as a continuation of the thread that has established 
        /// the awaited condition. The main advantage of the above solution could be revealed when the program validity is proved because the monitor is not released at all, 
        /// and thereby there is no possibility to change the data enclosed by the monitor, and the established condition as well, by another, third process.
        /// </remarks>
        void Send();

        /// <summary>
        /// A thread that cannot proceed because an event is not met will wait on a signal variable.
        /// </summary>
        /// <remarks>
        /// After invoking the wait operation the current process releases all the monitors that it possesses, thus it must leave all relevant data in a consistent state. 
        /// There must be initiated a scheduling mechanism to choose another process to run because the processor is released as well.
        /// </remarks>
        void Wait();

        /// <summary>
        /// Check if any thread is waiting for the specified signal
        /// </summary>
        bool Await();
    }
}
