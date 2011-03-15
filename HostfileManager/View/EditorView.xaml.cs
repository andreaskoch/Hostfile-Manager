namespace HostfileManager.View
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using HostfileManager.Model;
    using HostfileManager.Model.Base;
    using HostfileManager.UI.Controls;
    using HostfileManager.ViewModel;

    /// <summary>
    /// The <see cref="EditorView"/> view allows
    /// the user to edtit the host files visually.
    /// </summary>
    public partial class EditorView
    {
        #region public fields / ui-commands

        /// <summary>The restore-default hostfile content command.</summary>
        public static readonly RoutedCommand RestoreDefaultCommand = new RoutedCommand();

        /// <summary>The reload-from-disk command.</summary>
        public static readonly RoutedCommand ReloadFromDiskCommand = new RoutedCommand();

        /// <summary>The clear-hosts comamnd.</summary>
        public static readonly RoutedCommand ClearHostsCommand = new RoutedCommand();

        /// <summary>The save-changes command.</summary>
        public static readonly RoutedCommand SaveChangesCommand = new RoutedCommand();

        /// <summary>The select treeview item command.</summary>
        public static readonly RoutedCommand SelectTreeViewItemCommand = new RoutedCommand();

        /// <summary>The move entry up command.</summary>
        public static readonly RoutedCommand MoveUpCommand = new RoutedCommand();

        /// <summary>The move entry down command.</summary>
        public static readonly RoutedCommand MoveDownCommand = new RoutedCommand();

        /// <summary>The edit command.</summary>
        public static readonly RoutedCommand EditCommand = new RoutedCommand();

        /// <summary>The toggle active-state command.</summary>
        public static readonly RoutedCommand ToggleActiveStateCommand = new RoutedCommand();

        /// <summary>The delete command.</summary>
        public static readonly RoutedCommand DeleteCommand = new RoutedCommand();

        /// <summary>The add-group command</summary>
        public static readonly RoutedCommand AddGroupCommand = new RoutedCommand();

        /// <summary>The add-host command.</summary>
        public static readonly RoutedCommand AddHostCommand = new RoutedCommand();

        /// <summary>The add-domain command.</summary>
        public static readonly RoutedCommand AddDomainCommand = new RoutedCommand();

        /// <summary>The add-comment command.</summary>
        public static readonly RoutedCommand AddCommentCommand = new RoutedCommand();

        #endregion

        #region constructor

        /// <summary>Initializes a new instance of the <see cref="EditorView"/> class.</summary>
        public EditorView()
        {
            InitializeComponent();
            this.DataContextChanged += this.HostsFileGroupEditorViewDataContextChanged;
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
            if (this.ViewModel != null)
            {
                this.ViewModel.ReloadFromDisk();   
            }
        }

        #endregion

        #region clear hosts

        /// <summary>Check if the <see cref="CommandBindingClearHostsExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingClearHostsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ViewModel != null && this.ViewModel.HostFileInstance != null && this.ViewModel.HostFileInstance.Childs.Count > 0;
        }

        /// <summary>
        /// Remove all hosts groups.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingClearHostsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.ViewModel != null)
            {
                this.ViewModel.Clear();
            }
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
        /// Save all changes made to the current view.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingSaveChangesExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.SaveToDisk();
        }

        #endregion

        #region delete entry

        /// <summary>Check if the <see cref="CommandBindingDeleteExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingDeleteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            TreeView source = e.Source as TreeView;

            IHostfileEntry entry;
            if ((entry = e.Parameter as IHostfileEntry) == null && source != null)
            {
                entry = source.SelectedItem as IHostfileEntry;
            }
            
            e.CanExecute = entry != null && this.ViewModel != null;
        }

        /// <summary>
        /// Delete an entry.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingDeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TreeView source = e.Source as TreeView;

            IHostfileEntry entry;
            if ((entry = e.Parameter as IHostfileEntry) == null && source != null)
            {
                entry = source.SelectedItem as IHostfileEntry;
            }

            if (this.ViewModel != null && entry != null)
            {
                this.ViewModel.Remove(entry);
            }
        }

        #endregion

        #region select entry

        /// <summary>Check if the <see cref="CommandBindingSelectTreeViewItemExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingSelectTreeViewItemCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            TreeView source = e.Source as TreeView;
            e.CanExecute = source != null && e.Parameter != null;
        }

        /// <summary>
        /// Mark the current mouse click target as the selected item of the treeview.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingSelectTreeViewItemExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TreeView source = e.Source as TreeView;
            HostfileEntry entry = e.Parameter as HostfileEntry;
            if (source != null && entry != null)
            {
                entry.IsSelected = true;
            }
        }

        #endregion

        #region move entry up

        /// <summary>Check if the <see cref="CommandBindingMoveUpExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingMoveUpCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            TreeView source = e.Source as TreeView;

            IHostfileEntry entry;
            if ((entry = e.Parameter as IHostfileEntry) == null && source != null)
            {
                entry = source.SelectedItem as IHostfileEntry;
            }

            e.CanExecute = entry != null && this.ViewModel != null;
        }

        /// <summary>
        /// Move the current <see cref="HostfileEntry"/> up.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingMoveUpExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TreeView source = e.Source as TreeView;

            IHostfileEntry entry;
            if ((entry = e.Parameter as IHostfileEntry) == null && source != null)
            {
                entry = source.SelectedItem as IHostfileEntry;
            }

            if (this.ViewModel != null && entry != null)
            {
                this.ViewModel.MoveUp(entry);
            }
        }

        #endregion

        #region move entry down

        /// <summary>Check if the <see cref="CommandBindingMoveDownExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingMoveDownCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            TreeView source = e.Source as TreeView;

            IHostfileEntry entry;
            if ((entry = e.Parameter as IHostfileEntry) == null && source != null)
            {
                entry = source.SelectedItem as IHostfileEntry;
            }

            e.CanExecute = entry != null && this.ViewModel != null;
        }

        /// <summary>
        /// Move the current <see cref="HostfileEntry"/> down.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingMoveDownExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TreeView source = e.Source as TreeView;

            IHostfileEntry entry;
            if ((entry = e.Parameter as IHostfileEntry) == null && source != null)
            {
                entry = source.SelectedItem as IHostfileEntry;
            }

            if (this.ViewModel != null && entry != null)
            {
                this.ViewModel.MoveDown(entry);
            }
        }

        #endregion

        #region edit entry

        /// <summary>Check if the <see cref="CommandBindingEditExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingEditCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            TreeView source = e.Source as TreeView;
            EditableTextBlock textBlock = e.OriginalSource as EditableTextBlock;

            e.CanExecute = source != null && textBlock != null;
        }

        /// <summary>
        /// Edit the current <see cref="IHostfileEntry"/> object.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingEditExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TreeView source = e.Source as TreeView;
            EditableTextBlock textBlock = e.OriginalSource as EditableTextBlock;

            if (source != null && textBlock != null)
            {
                textBlock.IsInEditMode = !textBlock.IsInEditMode;
            }
        }

        #endregion

        #region toggle active state

        /// <summary>Check if the <see cref="CommandBindingToggleActiveStateExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingToggleActiveStateCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            TreeView source = e.Source as TreeView;

            ActivatableHostfileEntry activatableHostfileEntry;
            if ((activatableHostfileEntry = e.Parameter as ActivatableHostfileEntry) == null && source != null)
            {
                activatableHostfileEntry = source.SelectedItem as ActivatableHostfileEntry;
            }

            e.CanExecute = activatableHostfileEntry != null && this.ViewModel != null;
        }

        /// <summary>
        /// Toggle the status of the current <see cref="Host"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingToggleActiveStateExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TreeView source = e.Source as TreeView;

            ActivatableHostfileEntry activatableHostfileEntry;
            if ((activatableHostfileEntry = e.Parameter as ActivatableHostfileEntry) == null && source != null)
            {
                activatableHostfileEntry = source.SelectedItem as ActivatableHostfileEntry;
            }

            if (this.ViewModel != null && activatableHostfileEntry != null)
            {
                this.ViewModel.ToggleActiveState(activatableHostfileEntry);
            }
        }

        #endregion

        #region add group

        /// <summary>Check if the <see cref="CommandBindingAddGroupExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingAddGroupCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ViewModel != null;
        }

        /// <summary>
        /// Add a new <see cref="HostGroup"/> object.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingAddGroupExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.ViewModel != null)
            {
                this.ViewModel.AddGroup();
            }
        }

        #endregion

        #region add host

        /// <summary>Check if the <see cref="CommandBindingAddHostExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingAddHostCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ViewModel != null;
        }

        /// <summary>
        /// Add a new <see cref="Host"/> object.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingAddHostExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            HostGroup parent = e.Parameter as HostGroup;
            if (this.ViewModel != null)
            {
                this.ViewModel.AddHost(parent);
            }
        }

        #endregion

        #region add domain

        /// <summary>Check if the <see cref="CommandBindingAddDomainExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingAddDomainCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Host parent = e.Parameter as Host;
            e.CanExecute = this.ViewModel != null && parent != null;
        }

        /// <summary>
        /// Add a new <see cref="Domain"/> object.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingAddDomainExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Host parent = e.Parameter as Host;
            if (this.ViewModel != null && parent != null)
            {
                this.ViewModel.AddDomain(parent);
            }
        }

        #endregion

        #region add comment

        /// <summary>Check if the <see cref="CommandBindingAddCommentExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingAddCommentCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ViewModel != null;
        }

        /// <summary>
        /// Add a new comment.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingAddCommentExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.ViewModel != null)
            {
                this.ViewModel.AddComment();
            }
        }

        #endregion

        #endregion

        #region private functions and methods

        /// <summary>
        /// Occurs when the data context for this element changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="DependencyPropertyChangedEventArgs"/> that contains the event data.</param> 
        private void HostsFileGroupEditorViewDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.ViewModel == null)
            {
                return;
            }

            this.ViewModel.Save += this.GroupEditorViewSave;
            this.ViewModel.Copy += this.GroupEditorViewCopyToClipboard;
            this.ViewModel.Reload += this.GroupEditorViewReloadFromDisk;
        }

        /// <summary>
        /// Saves the changes made to this view.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ViewModeEventArgs"/> that contains the event data.</param>
        private void GroupEditorViewSave(object sender, ViewModeEventArgs e)
        {
            if (e != null && e.SourceView.Equals(ViewMode.Editor))
            {
                this.ViewModel.SaveToDisk(e.TargetPath);
            }
        }

        /// <summary>
        /// Copy the current application state to the user's clipboard.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ViewModeEventArgs"/> that contains the event data.</param>
        private void GroupEditorViewCopyToClipboard(object sender, ViewModeEventArgs e)
        {
            if (e == null || e.SourceView.Equals(ViewMode.Editor) == false)
            {
                return;
            }

            string text = this.ViewModel.GetText();
            Clipboard.SetData(DataFormats.Text, text);
        }

        /// <summary>
        /// Discard all changes and reload the current hosts file from disk.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ViewModeEventArgs"/> that contains the event data.</param>
        private void GroupEditorViewReloadFromDisk(object sender, ViewModeEventArgs e)
        {
            if (e == null || e.SourceView.Equals(ViewMode.Editor) == false || this.ViewModel == null)
            {
                return;
            }

            this.ViewModel.ReloadFromDisk();
        }

        #endregion
    }
}
