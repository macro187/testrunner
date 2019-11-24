using System;
using TestRunner.Events;
using TestRunner.Infrastructure;

namespace TestRunner.EventHandlers
{

    /// <summary>
    /// Event handler that outputs events in a single-line machine-readable JSON-based text format
    /// </summary>
    ///
    public class MachineOutputEventHandler : EventHandler
    {

        public override void Handle(TestRunnerEvent e)
        {
            Guard.NotNull(e, nameof(e));
            Console.Out.WriteLine(MachineReadableEventSerializer.Serialize(e));
            base.Handle(e);
        }

    }
}
