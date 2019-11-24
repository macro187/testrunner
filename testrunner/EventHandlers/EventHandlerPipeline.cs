using System;
using TestRunner.Events;
using TestRunner.Infrastructure;

namespace TestRunner.EventHandlers
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
    public static class EventHandlerPipeline
    {

        static EventHandlerPipeline()
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
        /// <returns>
        /// An <see cref="IDisposable"/> that, when disposed, removes <paramref name="handler"/> from the pipeline
        /// </returns>
        ///
        public static IDisposable Prepend(EventHandler handler)
        {
            Guard.NotNull(handler, nameof(handler));
            handler.Next = First;
            First = handler;
            if (Last == null) Last = handler;
            return new Disposable(() => Remove(handler));
        }


        /// <summary>
        /// Add an event handler to the end of the pipeline
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="IDisposable"/> that, when disposed, removes <paramref name="handler"/> from the pipeline
        /// </returns>
        ///
        public static IDisposable Append(EventHandler handler)
        {
            Guard.NotNull(handler, nameof(handler));
            if (First == null) First = handler;
            if (Last != null) Last.Next = handler;
            Last = handler;
            return new Disposable(() => Remove(handler));
        }


        static void Remove(EventHandler handler)
        {
            Guard.NotNull(handler, nameof(handler));

            for (EventHandler h = First, prev = null; h != null; prev = h, h = h.Next)
            {
                if (h != handler) continue;

                if (handler == First)
                {
                    First = handler.Next;
                }

                if (prev != null)
                {
                    prev.Next = handler.Next;
                }

                if (handler == Last)
                {
                    Last = prev;
                }

                handler.Next = null;
            }
        }

    }
}
