

using System;

namespace Prism.Interactivity.InteractionRequest
{
    /// <summary>
    /// Event args for the <see cref="IInteractionRequest.Raised"/> event.
    /// </summary>
    public class InteractionRequestedEventArgs : EventArgs
    {
        public InteractionRequestedEventArgs() { }

        public InteractionRequestedEventArgs(Object paramter)
        {
            this.Parameter = paramter;
        }

        public InteractionRequestedEventArgs(Object paramter, Action<Object> callback)
        {
            this.Parameter = paramter;
            this.Callback = callback;
        }

        public Object Parameter { get; private set; }

        public Action<Object> Callback { get; private set; }
    }
}
