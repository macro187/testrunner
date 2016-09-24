using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TestRunner
{
    public class Stats
    {
        private int globalFailCount;
        private int globalPassCount;
        private int globalCount;
        private Stopwatch globalTime;
        private Stopwatch localTime;

        public int GlobalFailCount
        {
            get
            {
                return globalFailCount;
            }
        }

        public int GlobalPassCount
        {
            get
            {
                return globalPassCount;
            }
        }

        public int GlobalCount {
            get
            {
                return globalCount;
            }
        }

        public TimeSpan GlobalTime
        {
            get
            {
                return globalTime.Elapsed;
            }
        }

        public TimeSpan LocalTime
        {
            get
            {
                return localTime.Elapsed;
            }
        }

        public Stats()
        {
            globalTime = new Stopwatch();
            localTime = new Stopwatch();

            globalTime.Start();
        }

        public void AddGlobalFailCount()
        {
            globalFailCount++;
        }

        public void AddGlobalPassCount()
        {
            globalPassCount++;
        }

        public void AddGlobalCount()
        {
            globalCount++;
        }

        public void StartLocalTime()
        {
            localTime.Start();
        }

        public void ResetLocalTime()
        {
            localTime.Reset();
        }

        public string GetFinalResult()
        {
            localTime.Stop();
            globalTime.Stop();

            var sb = new StringBuilder();
            sb.AppendLine("Ran:     " + globalCount.ToString() + " tests");
            sb.AppendLine("Passed:  " + globalPassCount.ToString() + " tests");
            sb.AppendLine("Failed:  " + globalFailCount.ToString() + " tests");
            sb.AppendLine("Elapsed: " + GlobalTime.TotalMilliseconds.ToString("N0") + " ms");
            return sb.ToString();
        }
    }
}
