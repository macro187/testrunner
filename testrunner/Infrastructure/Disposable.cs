using System;

namespace TestRunner.Infrastructure
{

    /// <summary>
    /// An <see cref="IDisposable"/> that performs an action when disposed
    /// </summary>
    ///
    public class Disposable : IDisposable
    {

        public Disposable(Action disposeAction)
        {
            Guard.NotNull(disposeAction, nameof(disposeAction));
            this.disposeAction = disposeAction;
        }


        Action disposeAction;
        bool disposed;


        void IDisposable.Dispose()
        {
            if (disposed) return;
            disposeAction();
            disposed = true;
        }

    }

}
