using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorImplementation.HoareMonitor
{
    public interface ICondition
    {
        /// <summary>
        /// A thread that changes the state of the monitor in a way that might allow a waiting thread to proceed will signal the condition variable and, 
        /// as a result, awake up one of the waiting threads.
        /// </summary>
        /// <remarks>
        /// This operation is based upon the principle that the signaling thread keeps control of the monitor, and the signaled one changes only its state and becomes ready to run. 
        /// Of course, it cannot be assumed that the announced condition is still fulfilled when the signaled process is resumed, because other processes, 
        /// taking precedence, may have changed it in the meantime. Therefore, the signaled process, just after taking the control, should check again the condition, 
        /// except that it cannot be changed, and, if necessary, wait once more for its occurrence.
        /// </remarks>
        void Send();

        /// <summary>
        /// A thread that cannot proceed because an event is not met will wait on a condition variable.
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
