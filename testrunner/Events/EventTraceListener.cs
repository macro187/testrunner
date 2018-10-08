using System.Diagnostics;

namespace TestRunner.Events
{

    /// <summary>
    /// A <see cref="TraceListener"/> that redirects output to the <see cref="EventHandler"/> pipeline
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
                EventHandlers.First.Handle(new TraceOutputEvent() { Message = line });

                message = message.Substring(i + 1);
            }

            buffer = buffer + message;
        }

    }
}
