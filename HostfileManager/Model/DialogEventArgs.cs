namespace HostfileManager.Model
{
    using System;

    /// <summary>Event arguments for the <see cref="DialogEventArgs"/> class.</summary>
    public class DialogEventArgs : EventArgs
    {
        #region constructor(s)

        /// <summary>Initializes a new instance of the <see cref="DialogEventArgs"/> class.</summary>
        /// <param name="actionType">The action type.</param>
        public DialogEventArgs(DialogActionType actionType)
        {
            this.ActionType = actionType;
        }

        #endregion

        #region properties

        /// <summary>Gets <see cref="DialogActionType"/> for this <see cref="DialogEventArgs"/> object instance.</summary>
        public DialogActionType ActionType { get; private set; }

        #endregion
    }
}