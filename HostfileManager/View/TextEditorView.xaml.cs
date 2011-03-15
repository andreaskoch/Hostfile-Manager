namespace HostfileManager.View
{
    using System.Windows;
    using System.Windows.Input;
    using HostfileManager.Model;
    using HostfileManager.ViewModel;

    /// <summary>
    /// The <see cref="TextEditorView"/> view
    /// offers controls for editing hosts files through
    /// a simple text-editor.
    /// </summary>
    public partial class TextEditorView
    {
        #region ui-commands

        /// <summary>The initialize text-editor command.</summary>
        public static readonly RoutedCommand InitializeTextEditorCommand = new RoutedCommand();

        /// <summary>The clear text-editor content command.</summary>
        public static readonly RoutedCommand ClearTextEditorContentCommand = new RoutedCommand();

        /// <summary>The restore-default hostfile content command.</summary>
        public static readonly RoutedCommand RestoreDefaultCommand = new RoutedCommand();

        /// <summary>The reload-from-disk command.</summary>
        public static readonly RoutedCommand ReloadFromDiskCommand = new RoutedCommand();

        /// <summary>The save-changes command.</summary>
        public static readonly RoutedCommand SaveChangesCommand = new RoutedCommand();

        #endregion

        #region constructor

        /// <summary>Initializes a new instance of the <see cref="TextEditorView"/> class.</summary>
        public TextEditorView()
        {
            InitializeComponent();
            this.DataContextChanged += this.HostsFileTextEditorViewDataContextChanged;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the current ViewModel.
        /// </summary>
        public HostsFileViewModel ViewModel
        {
            get
            {
                return this.DataContext as HostsFileViewModel;
            }
        }

        /// <summary>
        /// Gets an instance of the current hosts file.
        /// </summary>
        public HostFile HostfileInstance
        {
            get
            {
                return this.ViewModel != null ? this.ViewModel.HostFileInstance : null;
            }
        }

        #endregion

        #region interface events

        #region initialize text editor

        /// <summary>Check if the <see cref="CommandBindingInitializeTextEditorExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingInitializeTextEditorCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ViewModel != null && this.ViewModel.HostFileInstance != null && string.IsNullOrEmpty(this.txtHostsFileContent.Text);
        }

        /// <summary>
        /// Initialize the text-editor by adding a dummy comment.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingInitializeTextEditorExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.ViewModel != null && this.ViewModel.HostFileInstance != null && string.IsNullOrEmpty(this.txtHostsFileContent.Text))
            {
                string parameterText = "# ";
                if (e.Parameter != null)
                {
                    parameterText = e.Parameter as string;
                }

                this.txtHostsFileContent.Text = parameterText;
            }
        }

        #endregion

        #region clear text-editor content

        /// <summary>Check if the <see cref="CommandBindingClearContentExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingClearContentCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ViewModel != null && this.ViewModel.HostFileInstance != null && string.IsNullOrEmpty(this.txtHostsFileContent.Text) == false;
        }

        /// <summary>
        /// Clears the text-editor content.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingClearContentExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.ViewModel != null && this.ViewModel.HostFileInstance != null)
            {
                this.ViewModel.HostFileInstance.Clear();
                this.txtHostsFileContent.Text = string.Empty;
            }
        }

        #endregion

        #region restore default

        /// <summary>Check if the <see cref="CommandBindingRestoreDefaultHostsFileExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingRestoreDefaultHostsFileCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ViewModel != null;
        }

        /// <summary>
        /// Restore the default host-file content.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingRestoreDefaultHostsFileExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.ViewModel != null)
            {
                this.ViewModel.RestoreDefaultHostsFile();
            }
        }

        #endregion

        #region reload from disk

        /// <summary>Check if the <see cref="CommandBindingReloadFromDiskExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingReloadFromDiskCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ViewModel != null && this.ViewModel.HostFileInstance != null;
        }

        /// <summary>
        /// Dismiss all changes and reload the the hosts-file data from disk.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingReloadFromDiskExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.ReloadFromDisk();
        }

        #endregion

        #region save changes

        /// <summary>Check if the <see cref="CommandBindingSaveChangesExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingSaveChangesCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ViewModel != null;
        }

        /// <summary>
        /// Saves the changes made to this view.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingSaveChangesExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.SaveChanges(this.ViewModel.HostFileInstance.FilePath);
        }

        #endregion

        #endregion

        #region private functions and methods

        /// <summary>
        /// Occurs when the data context for this element changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="DependencyPropertyChangedEventArgs"/> that contains the event data.</param> 
        private void HostsFileTextEditorViewDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.ViewModel == null)
            {
                return;
            }

            this.ViewModel.Save += this.TextEditorViewSave;
            this.ViewModel.Copy += this.TextEditorViewCopyToClipboard;
            this.ViewModel.Reload += this.TextEditorReloadFromDisk;
        }

        /// <summary>
        /// Saves the changes made to this view.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ViewModeEventArgs"/> that contains the event data.</param>
        private void TextEditorViewSave(object sender, ViewModeEventArgs e)
        {
            if (e != null && e.SourceView.Equals(ViewMode.TextEditor))
            {
                this.SaveChanges(e.TargetPath);
            }
        }

        /// <summary>
        /// Copy the current application state to the user's clipboard.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ViewModeEventArgs"/> that contains the event data.</param>
        private void TextEditorViewCopyToClipboard(object sender, ViewModeEventArgs e)
        {
            if (e != null && e.SourceView.Equals(ViewMode.TextEditor))
            {
                Clipboard.SetData(DataFormats.Text, this.txtHostsFileContent.Text);
            }
        }

        /// <summary>
        /// Discard all changes and reload the current hosts file from disk.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ViewModeEventArgs"/> that contains the event data.</param>
        private void TextEditorReloadFromDisk(object sender, ViewModeEventArgs e)
        {
            if (e == null || e.SourceView.Equals(ViewMode.TextEditor) == false || this.ViewModel == null)
            {
                return;
            }

            this.ViewModel.ReloadFromDisk();
        }

        /// <summary>Save the changes that have been made to the current view.</summary>
        /// <param name="filepath">The filepath.</param>
        private void SaveChanges(string filepath)
        {
            this.ViewModel.SaveToDisk(filepath, this.txtHostsFileContent.Text);
        }

        #endregion
    }
}
