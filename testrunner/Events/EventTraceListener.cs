using System.Diagnostics;

namespace TestRunner.Events
{

    /// <summary>
    /// A <see cref="System.Diagnostics.TraceListener"/> that redirects output to <see cref="EventHandler"/>
    /// </summary>
    ///
    public class EventTraceListener : TraceListener
    {

        string buffer = "";


        public override void WriteLine(string message)
        {
            Write(message + "\n");
        }


        public override void Write(string message)
        {
            message = message ?? "";
            message = message.Replace("\r\n", "\n").Replace("\r", "\n");

            int i;
            while ((i = message.IndexOf('\n')) >= 0)
            {
                var line = message.Substring(0, i);
                if (buffer != "")
                {
                    line = buffer + line;
                    buffer = "";
                }
                EventHandler.TraceEvent(line);

                message = message.Substring(i + 1);
            }

            buffer = buffer + message;
        }

    }
}
