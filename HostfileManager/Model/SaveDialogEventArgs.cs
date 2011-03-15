namespace HostfileManager.Model
{
    using System;

    using HostfileManager.UI.Dialogs;

    /// <summary>Event arguments for the <see cref="SaveProfileDialog"/> class.</summary>
    /// <typeparam name="T">The argument type.</typeparam>
    public class SaveDialogEventArgs<T> : EventArgs
    {
        #region constructor(s)

        /// <summary>Initializes a new instance of the <see cref="SaveDialogEventArgs{T}"/> class.</summary>
        /// <param name="actionType">The action type.</param>
        public SaveDialogEventArgs(SaveDialogActionType actionType)
        {
            this.ActionType = actionType;
        }

        /// <summary>Initializes a new instance of the <see cref="SaveDialogEventArgs{T}"/> class.</summary>
        /// <param name="actionType">The action type.</param>
        /// <param name="returnvalue">The return value.</param>
        public SaveDialogEventArgs(SaveDialogActionType actionType, T returnvalue)
        {
            this.ActionType = actionType;
            this.ReturnValue = returnvalue;
        }

        #endregion

        #region properties

        /// <summary>Gets <see cref="SaveDialogActionType"/> for this <see cref="SaveDialogEventArgs{T}"/> object instance.</summary>
        public SaveDialogActionType ActionType { get; private set; }

        /// <summary>Gets return value for this <see cref="SaveDialogEventArgs{T}"/> object instance.</summary>
        public T ReturnValue { get; private set; }

        #endregion
    }
}