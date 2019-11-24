using System;
using System.Collections;
using System.Collections.Generic;
using TestRunner.Infrastructure;

namespace TestRunner.Results
{
    public class ExceptionInfo
    {

        public ExceptionInfo(Exception ex)
            : this()
        {
            ParseException(ex);
        }


        public ExceptionInfo()
        {
            Data = new Dictionary<string, string>();
            StackTrace = new List<StackFrameInfo>();
        }


        void ParseException(Exception ex)
        {
            Guard.NotNull(ex, nameof(ex));

            FullName = ex.GetType().FullName;
            Message = ex.Message;
            Source = ex.Source;
            HelpLink = ex.HelpLink;
            ParseData(ex.Data);
            ParseStackTrace(ex.StackTrace);
            if (ex.InnerException != null) InnerException = new ExceptionInfo(ex.InnerException);
        }


        void ParseData(IDictionary data)
        {
            if (data == null) return;
            foreach (DictionaryEntry de in data)
            {
                Data.Add(Convert.ToString(de.Key), Convert.ToString(de.Value));
            }
        }


        void ParseStackTrace(string stackTrace)
        {
            if (stackTrace == null) return;
            var lines = StringExtensions.SplitLines(stackTrace);
            foreach (var line in lines)
            {
                ParseStackFrame(line);
            }
        }


        void ParseStackFrame(string line)
        {
            var atpart = line.Trim();
            if (atpart == "") return;
            if (atpart.StartsWith("at ", StringComparison.Ordinal))
            {
                atpart = atpart.Substring(3);
            }

            var inpart = "";
            var inpos = atpart.IndexOf(" in ", StringComparison.Ordinal);
            if (inpos >= 0)
            {
                inpart = atpart.Substring(inpos + 4);
                atpart = atpart.Substring(0, inpos);
            }

            StackTrace.Add(new StackFrameInfo() { At = atpart, In = inpart });
        }


        public string FullName { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string HelpLink { get; set; }
        public Dictionary<string, string> Data;
        public List<StackFrameInfo> StackTrace { get; set; }
        public ExceptionInfo InnerException { get; set; }

    }
}
