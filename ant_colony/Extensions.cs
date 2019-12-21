using System.Collections.Generic;
using System.Threading;
namespace Extensions
{
    public static class ThreadExtension
    {
        public static void WaitAll(this List<Thread> threads)
        {
            if (threads != null)
            {
              
                foreach (Thread thread in threads)
                {
                    thread.Start();
                    thread.Join();
                }
            }
        }
    }
}