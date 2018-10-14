using TestRunner.Infrastructure;

namespace TestRunner.Events
{

    /// <summary>
    /// Event handler pipeline
    /// </summary>
    ///
    /// <remarks>
    /// The TestRunner program raises events as it runs.  Each handler in the pipeline has the opportunity to perform
    /// actions and/or modify the event before propagating to the next one.
    /// </remarks>
    ///
    public static class EventHandlers
    {

        static EventHandlers()
        {
            Append(new EventHandler());
        }


        private static EventHandler First;


        private static EventHandler Last;


        /// <summary>
        /// Raise an event
        /// </summary>
        ///
        public static void Raise(TestRunnerEvent e)
        {
            First.Handle(e);
        }


        /// <summary>
        /// Add an event handler to the beginning of the pipeline
        /// </summary>
        ///
        public static void Prepend(EventHandler handler)
        {
            Guard.NotNull(handler, nameof(handler));
            handler.Next = First;
            First = handler;
            if (Last == null) Last = handler;
        }


        /// <summary>
        /// Add an event handler to the end of the pipeline
        /// </summary>
        ///
        public static void Append(EventHandler handler)
        {
            Guard.NotNull(handler, nameof(handler));
            if (First == null) First = handler;
            if (Last != null) Last.Next = handler;
            Last = handler;
        }

    }
}
