using Avalonia.Xaml.Interactions.Core;
using System;

namespace Prism.Interactivity.InteractionRequest
{
    /// <summary>
    /// Custom event trigger for using with <see cref="IInteractionRequest"/> objects.
    /// </summary>
    /// <remarks>
    /// The standard <see cref="System.Windows.Interactivity.EventTrigger"/> class can be used instead, as long as the 'Raised' event 
    /// name is specified.
    /// </remarks>
    public class InteractionRequestTriggerBehavior : EventTriggerBehavior
    {
        public InteractionRequestTriggerBehavior()
        {
            EventName = "Raised";
        }
    }
}
