using System;
using System.Diagnostics;

namespace Multiplayer_Server
{
    public class HiResTimer
    {
        private Stopwatch stopwatch;

        public HiResTimer()
        {
            stopwatch = new Stopwatch();
        }
        public void Start()
        {
            if (stopwatch.IsRunning)
            {
                //Console.WriteLine("Stopwatch is already stopped");
            }
            else
            {
                stopwatch.Start();
            }
        }
        public void Stop()
        {
            if (!stopwatch.IsRunning)
            {
                //Console.WriteLine("Stopwatch is already stopped");
            }
            else
            {
                stopwatch.Stop();
            }
        }
        public void Reset()
        {
            stopwatch.Reset();
        }
        public float Duration()
        {
            double temp = Convert.ToDouble(stopwatch.ElapsedMilliseconds);
            return (float)temp * 5.0f;
        }
    }
}