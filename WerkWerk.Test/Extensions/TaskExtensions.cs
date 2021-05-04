using System.Threading.Tasks;

namespace WerkWerk.Test
{
    public static class TaskExtensions
    {
        public static void Fire(this Task t)
        {
            if (t.Status == TaskStatus.WaitingToRun)
            {
                t.Start();
            }
        }
    }
}