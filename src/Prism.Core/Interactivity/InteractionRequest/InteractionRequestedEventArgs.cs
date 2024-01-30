

using System;

namespace Prism.Interactivity.InteractionRequest
{
    /// <summary>
    /// Event args for the <see cref="IInteractionRequest.Raised"/> event.
    /// </summary>
    public class InteractionRequestedEventArgs<T, K> : EventArgs
    {
        public InteractionRequestedEventArgs() { }

        public InteractionRequestedEventArgs(T paramter)
        {
            this.Parameter = paramter;
        }

        public InteractionRequestedEventArgs(T paramter, Action<K> callback)
        {
            this.Parameter = paramter;
            this.Callback = callback;
        }

        public T Parameter { get; private set; }

        public Action<K> Callback { get; private set; }
    }
}
