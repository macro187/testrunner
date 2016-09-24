using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Brick.MSTestRunner
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
            StringBuilder result = new StringBuilder();
            localTime.Stop();
            globalTime.Stop();

            result.AppendFormat("{0} test(s) executed.", globalCount);
            result.Append(Environment.NewLine);
            result.AppendFormat("{0} passed and {1} failed.", 
                globalPassCount,
                globalFailCount);
            result.Append(Environment.NewLine);
            result.AppendFormat("Total time elapsed: {0} s.",
                GlobalTime.TotalSeconds);

            return result.ToString();
        }
    }
}
