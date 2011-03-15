﻿namespace HostfileManager.UI.Dialogs
{
    using System;
    using System.Windows.Input;

    using HostfileManager.Model;

    /// <summary>
    /// The <see cref="Dialog"/> class displays a message to the user.
    /// </summary>
    public partial class Dialog
    {
        #region public commands

        /// <summary>The confirm comamnd.</summary>
        public static readonly RoutedCommand ConfirmCommand = new RoutedCommand();

        /// <summary>The cancel command.</summary>
        public static readonly RoutedCommand CancelCommand = new RoutedCommand();

        #endregion

        #region constructor(s)

        /// <summary>Initializes a new instance of the <see cref="Dialog"/> class.</summary>
        public Dialog()
        {
            InitializeComponent();
        }

        /// <summary>Initializes a new instance of the <see cref="Dialog"/> class.</summary>
        /// <param name="confirm">The confirm event callback method.</param>
        public Dialog(EventHandler<DialogEventArgs> confirm)
        {
            InitializeComponent();

            this.Confirm += confirm;            
        }

        /// <summary>Initializes a new instance of the <see cref="Dialog"/> class.</summary>
        /// <param name="confirm">The confirm event callback method.</param>
        /// <param name="cancel">The cancel event callback method.</param>
        public Dialog(EventHandler<DialogEventArgs> confirm, EventHandler<DialogEventArgs> cancel)
        {
            InitializeComponent();

            this.Confirm += confirm;
            this.Cancel += cancel;
        }

        #endregion

        #region protected events

        /// <summary>The confirm event.</summary>
        protected event EventHandler<DialogEventArgs> Confirm;

        /// <summary>The cancel event.</summary>
        protected event EventHandler<DialogEventArgs> Cancel;

        #endregion

        #region public properties

        /// <summary>Gets or sets text of the dialog's message block.</summary>
        public string Message
        {
            get
            {
                return this.msgBlock.Text;
            }

            set
            {
                this.msgBlock.Text = value;
            }
        }

        /// <summary>Gets or sets text of the confirm-button.</summary>
        public string ConfirmButtonText
        {
            get
            {
                return this.btnConfirm.Content as string;
            }

            set
            {
                this.btnConfirm.Content = value;
            }
        }

        /// <summary>Gets or sets the text of the cancel-button.</summary>
        public string CancelButtonText
        {
            get
            {
                return this.btnCancel.Content as string;
            }

            set
            {
                this.btnCancel.Content = value;
            }
        }

        #endregion

        #region private ui events

        #region confirm

        /// <summary>Check if the <see cref="CommandBindingConfirmExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingConfirmCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Executes the <see cref="ConfirmMethod"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingConfirmExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.ConfirmMethod();
        }

        #endregion

        #region cancel

        /// <summary>Check if the <see cref="CommandBindingCancelExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingCancelCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Executes the <see cref="CancelMethod"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingCancelExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region closing

        /// <summary>
        /// Executes the <see cref="CancelMethod"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="System.ComponentModel.CancelEventArgs"/> that contains the event data.</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.CancelMethod();
        }

        #endregion

        #endregion

        #region private methods

        /// <summary>Excecute the confirm callback method.</summary>
        private void ConfirmMethod()
        {
            if (this.Confirm != null)
            {
                this.Confirm(this, new DialogEventArgs(DialogActionType.Confirm));
            }

            this.Close();
        }

        /// <summary>
        /// Excecute the cancel-callback method.
        /// </summary>
        private void CancelMethod()
        {
            if (this.Cancel != null)
            {
                this.Cancel(this, new DialogEventArgs(DialogActionType.Cancel));
            }
        }

        #endregion
    }
}
