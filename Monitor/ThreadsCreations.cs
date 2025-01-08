namespace MonitorImplementation.HoareMonitor
{
    public static class ThreadsCreations
    {
        public static Thread StartThread(ParameterizedThreadStart start)
        {
            Thread thread = new Thread(start);
            thread.Start();
            return thread;
        }

        public static void StartThreadsUsingThreadPool(WaitCallback start)
        {
            ThreadPool.QueueUserWorkItem(start);
        }

        public static Task StartThreadsUsingTask(Action start)
        {
            return Task.Run(start);
        }
    }
}