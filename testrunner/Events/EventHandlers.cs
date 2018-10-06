using TestRunner.Infrastructure;

namespace TestRunner.Events
{

    /// <summary>
    /// Event handler pipeline
    /// </summary>
    ///
    /// <remarks>
    /// The TestRunner program reports events to the <see cref="First"/> event handler in the pipeline as it runs.
    /// Each handler has the opportunity to perform actions before propagating to the next one.
    /// </remarks>
    ///
    public static class EventHandlers
    {

        static EventHandlers()
        {
            Append(new EventHandler());
        }


        /// <summary>
        /// The first event handler in the pipeline
        /// </summary>
        ///
        public static EventHandler First { get; private set; }


        private static EventHandler Last;


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
