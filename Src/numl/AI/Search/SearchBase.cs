using System;

namespace numl.AI.Search
{
    /// <summary>
    /// Class Search.
    /// </summary>
    public abstract class SearchBase<TState> where TState : class, IState
    {
        /// <summary>
        /// Occurs when [successor expanded].
        /// </summary>
        public event EventHandler<StateExpansionEventArgs> SuccessorExpanded;

        /// <summary>
        /// Handles the <see cref="E:SuccessorExpanded" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="StateExpansionEventArgs&lt;T&gt;"/> instance containing the event data.</param>
        protected virtual void OnSuccessorExpanded(object sender, StateExpansionEventArgs e)
        {
            EventHandler<StateExpansionEventArgs> handler = SuccessorExpanded;
            if (handler != null)
                handler(sender, e);
        }
    }

}