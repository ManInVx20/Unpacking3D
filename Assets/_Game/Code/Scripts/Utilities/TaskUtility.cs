using System;
using System.Threading.Tasks;

namespace VinhLB
{
    public static class TaskUtility
    {
        public static async Task WaitUntil(Func<bool> predicate, int sleep = 50)
        {
            while (!predicate())
            {
                await Task.Delay(sleep);
            }
        }
    }
}