namespace HostfileManager.View
{
    using System.Windows;
    using System.Windows.Input;

    using HostfileManager.Model;
    using HostfileManager.Model.Base;
    using HostfileManager.ViewModel;

    /// <summary>
    /// The <see cref="QuickToggleView"/> view allows
    /// the user to quickly activate or deactivate hostfile
    /// entries.
    /// </summary>
    public partial class QuickToggleView
    {
        #region ui-commands

        /// <summary>The Toggle-Item-Status command.</summary>
        public static readonly RoutedCommand ToggleItemStatusCommand = new RoutedCommand();

        /// <summary>
        /// The Load-Editor command.
        /// </summary>
        public static readonly RoutedCommand SwitchToEditorViewCommand = new RoutedCommand();

        /// <summary>
        /// The Load-TextEditor command.
        /// </summary>
        public static readonly RoutedCommand SwitchToTextEditorCommand = new RoutedCommand();

        #endregion

        #region constructor

        /// <summary>Initializes a new instance of the <see cref="QuickToggleView"/> class.</summary>
        public QuickToggleView()
        {
            InitializeComponent();
            this.DataContextChanged += this.HostsFileGroupViewDataContextChanged;
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
        public HostFile HostFileInstance
        {
            get
            {
                return this.ViewModel != null ? this.ViewModel.HostFileInstance : null;
            }
        }

        #endregion

        #region interface events

        #region toggle item status

        /// <summary>Check if the <see cref="CommandBindingToggleItemStatusExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingToggleItemStatusCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ViewModel != null && e.Parameter as ActivatableHostfileEntry != null;
        }

        /// <summary>
        /// Toggle the status of the currently selected <see cref="ActivatableHostfileEntry"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingToggleItemStatusExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ActivatableHostfileEntry activatableHostfileEntry;
            if ((activatableHostfileEntry = e.Parameter as ActivatableHostfileEntry) != null && this.ViewModel != null)
            {
                this.ViewModel.ToggleActiveState(activatableHostfileEntry);
                this.ViewModel.SaveToDisk(this.ViewModel.HostFileInstance.FilePath);
            }
        }

        #endregion

        #region load view: general

        /// <summary>Checks if the load-view functions can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>    
        private void CommandBindingSwitchViewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Application.Current != null)
            {
                var currentApplication = Application.Current as App;
                e.CanExecute = currentApplication != null;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        #endregion

        #region load view: Editor

        /// <summary>
        /// Switches the <see cref="ViewMode"/> to <see cref="ViewMode.Editor"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingSwitchToEditorExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (Application.Current != null)
            {
                var currentApplication = Application.Current as App;
                if (currentApplication != null)
                {
                    currentApplication.AppViewModel.SwitchToView(ViewMode.Editor);
                }
            }
        }

        #endregion

        #region load view: Text Editor

        /// <summary>
        /// Switches the <see cref="ViewMode"/> to <see cref="ViewMode.TextEditor"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingSwitchToTextEditorExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (Application.Current != null)
            {
                var currentApplication = Application.Current as App;
                if (currentApplication != null)
                {
                    currentApplication.AppViewModel.SwitchToView(ViewMode.TextEditor);
                }
            }
        }

        #endregion

        #endregion

        #region private methods and events

        /// <summary>Occurs when the data context for this element changes.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="DependencyPropertyChangedEventArgs"/> that contains the event data.</param>  
        private void HostsFileGroupViewDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.ViewModel == null)
            {
                return;
            }

            this.ViewModel.Save += this.GroupViewSave;
            this.ViewModel.Copy += this.GroupViewCopyToClipboard;
            this.ViewModel.Reload += this.GroupViewReloadFromDisk;
        }

        /// <summary>Saves the changes made to this view.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ViewModeEventArgs"/> that contains the event data.</param>  
        private void GroupViewSave(object sender, ViewModeEventArgs e)
        {
            if (e != null && e.SourceView.Equals(ViewMode.Overview))
            {
                this.ViewModel.SaveToDisk(e.TargetPath);
            }
        }

        /// <summary>
        /// Copy the current application state to the user's clipboard.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ViewModeEventArgs"/> that contains the event data.</param>
        private void GroupViewCopyToClipboard(object sender, ViewModeEventArgs e)
        {
            if (e == null || e.SourceView.Equals(ViewMode.Overview) == false)
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
        private void GroupViewReloadFromDisk(object sender, ViewModeEventArgs e)
        {
            if (e == null || e.SourceView.Equals(ViewMode.Overview) == false || this.ViewModel == null)
            {
                return;
            }
            
            this.ViewModel.ReloadFromDisk();
        }

        #endregion
    }
}
