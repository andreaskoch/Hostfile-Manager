namespace HostfileManager.UI
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;

    using HostfileManager.Logic;
    using HostfileManager.Model;
    using HostfileManager.UI.Dialogs;
    using HostfileManager.ViewModel;

    /// <summary>
    /// The <see cref="MainWindow"/> class is the main application window of the Hostfile Manager.
    /// </summary>
    public partial class MainWindow
    {
        #region public fields / ui-commands

        /// <summary>
        /// The Reload-From-Disk command.
        /// </summary>
        public static readonly RoutedCommand ReloadFromDiskCommand = new RoutedCommand();

        /// <summary>
        /// The Copy-To-Clipboard command.
        /// </summary>
        public static readonly RoutedCommand CopyToClipboardCommand = new RoutedCommand();
        
        /// <summary>
        /// The Save-Changes to hosts file command.
        /// </summary>
        public static readonly RoutedCommand SaveToHostsFileCommand = new RoutedCommand();
        
        /// <summary>
        /// The Save-Changes command.
        /// </summary>
        public static readonly RoutedCommand SaveChangesCommand = new RoutedCommand();

        /// <summary>
        /// The Load-Overview command.
        /// </summary>
        public static readonly RoutedCommand SwitchToOverviewCommand = new RoutedCommand();

        /// <summary>
        /// The Load-Editor command.
        /// </summary>
        public static readonly RoutedCommand SwitchToEditorViewCommand = new RoutedCommand();

        /// <summary>
        /// The Load-TextEditor command.
        /// </summary>
        public static readonly RoutedCommand SwitchToTextEditorCommand = new RoutedCommand();

        /// <summary>
        /// The Load-Specific-Profile command.
        /// </summary>
        public static readonly RoutedCommand LoadSpecificProfileCommand = new RoutedCommand();

        /// <summary>
        /// The Save-New-Profile command.
        /// </summary>
        public static readonly RoutedCommand SaveNewProfileCommand = new RoutedCommand();

        /// <summary>
        /// The Overwrite-Specific-Profile command.
        /// </summary>
        public static readonly RoutedCommand OverwriteSpecificProfileCommand = new RoutedCommand();

        /// <summary>
        /// The Delete-All-Profiles command.
        /// </summary>
        public static readonly RoutedCommand DeleteAllProfilesCommand = new RoutedCommand();

        /// <summary>
        /// The Delete-Specific-Profile command.
        /// </summary>
        public static readonly RoutedCommand DeleteSpecificProfileCommand = new RoutedCommand();

        /// <summary>
        /// The Application-Exit command.
        /// </summary>
        public static readonly RoutedCommand ExitCommand = new RoutedCommand();

        /// <summary>
        /// The Change Language command.
        /// </summary>
        public static readonly RoutedCommand ChangeUiLanguageCommand = new RoutedCommand();

        /// <summary>
        /// The Show-About-Application-Window command.
        /// </summary>
        public static readonly RoutedCommand ShowAboutBoxCommand = new RoutedCommand();

        #endregion

        #region private fields

        /// <summary>A flag indicating whether the <see cref="hardwareAccelerationIsDisabled"/> flag has already been set (default = false).</summary>
        private bool hardwareAccelerationIsDisabledHasBeenSet;

        /// <summary>A flag indicating whether this WPF Hardware Acceleration is disabled or not (default = false).</summary>
        private bool hardwareAccelerationIsDisabled;

        #endregion

        #region constructor(s)

        /// <summary>Initializes a new instance of the <see cref="MainWindow"/> class.</summary>
        public MainWindow()
        {
            InitializeComponent();

            /* attach icon to window */
            string applicationIconPath = App.GetApplicationResource("ApplicationIcon", string.Empty);
            if (string.IsNullOrEmpty(applicationIconPath) == false)
            {
                Uri iconUri = new Uri(applicationIconPath, UriKind.RelativeOrAbsolute);
                this.Icon = BitmapFrame.Create(iconUri);
            }

            /* add datacontext changed event listener */
            this.DataContextChanged += this.OnDataContextChanged;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the current ViewModel.
        /// </summary>
        public MainWindowViewModel ViewModel
        {
            get
            {
                return this.DataContext as MainWindowViewModel;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether (wpf) hardware acceleration is disabled.
        /// Note: If you want set this value, you need to set it BEFORE the <see cref="OnSourceInitialized"/> event is executed.
        /// </summary>
        public bool HardwareAccelerationIsDisabled
        {
            get
            {
                if (this.hardwareAccelerationIsDisabledHasBeenSet == false)
                {
                    var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
                    if (hwndSource != null && hwndSource.CompositionTarget != null)
                    {
                        this.hardwareAccelerationIsDisabled = hwndSource.CompositionTarget.RenderMode.Equals(RenderMode.SoftwareOnly);
                    }

                    this.hardwareAccelerationIsDisabledHasBeenSet = true;
                }

                return this.hardwareAccelerationIsDisabled;
            }

            protected set
            {
                this.hardwareAccelerationIsDisabled = value;
                this.hardwareAccelerationIsDisabledHasBeenSet = true;
            }
        }

        #endregion

        #region disable hardware acceleration

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.SourceInitialized"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            /* 
             * Fix WPF Window Repainting issues:
             * Disable hardware acceleration
             * URL: http://social.msdn.microsoft.com/Forums/en/wpf/thread/0a1873f5-5e8b-45c7-a921-d32aec8aa88e
             */
            if (this.HardwareAccelerationIsDisabled)
            {
                var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
                if (hwndSource != null && hwndSource.CompositionTarget != null)
                {
                    hwndSource.CompositionTarget.RenderMode = RenderMode.SoftwareOnly;
                }                
            }

            base.OnSourceInitialized(e);
        }

        #endregion

        #region application events

        #region save changes to hosts file

        /// <summary>Check if the <see cref="CommandBindingSaveToHostsFileExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingSaveToHostsFileCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ViewModel != null;
        }

        /// <summary>
        /// Save all changes to the user's hosts-file.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingSaveToHostsFileExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.SaveChangesToHostFile();
        }

        #endregion

        #region save changes to current file

        /// <summary>Check if the <see cref="CommandBindingSaveChangesExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingSaveChangesCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ViewModel != null;
        }

        /// <summary>
        /// Save all changes to the user's hosts-file.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingSaveChangesExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.SaveChangesToCurrentFile();
        }

        #endregion

        #region reload from disk

        /// <summary>Check if the <see cref="CommandBindingReloadFromDiskExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingReloadFromDiskCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ViewModel != null;
        }

        /// <summary>
        /// Discard all changes and reload the current hosts file from disk.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingReloadFromDiskExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.ReloadFromDisk();
        }

        #endregion

        #region copy to clipboard

        /// <summary>Check if the <see cref="CommandBindingCopyToClipboardExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingCopyToClipboardCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ViewModel != null;
        }

        /// <summary>
        /// Copy the text-representation of the current <see cref="HostFile"/> to the user's clipboard.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingCopyToClipboardExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.CopyToClipboard();
        }

        #endregion

        #region load specific sourceProfile

        /// <summary>Check if the <see cref="CommandBindingLoadSpecificProfileExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingLoadSpecificProfileCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            HostsFileProfile profile = e.Parameter as HostsFileProfile;
            if (this.ViewModel != null && profile != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        /// <summary>
        /// Load the selected <see cref="HostsFileProfile"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingLoadSpecificProfileExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            HostsFileProfile profile = e.Parameter as HostsFileProfile;
            if (this.ViewModel != null && profile != null)
            {
                this.ViewModel.LoadProfile(profile);
            }
        }

        #endregion

        #region save new profile

        /// <summary>Check if the <see cref="CommandBindingSaveNewProfileExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingSaveNewProfileCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ViewModel != null;
        }

        /// <summary>
        /// Save the current state to a new HostsFile-Profile.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingSaveNewProfileExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SaveProfileDialog saveDialog = new SaveProfileDialog(this.SaveNewProfileSuccess)
                {
                    Title = App.GetApplicationResource<string>("saveNewProfileDialogTitle"),
                    ConfirmButtonText = App.GetApplicationResource<string>("saveNewProfileDialogConfirmButtonText"),
                    CancelButtonText = App.GetApplicationResource<string>("saveNewProfileDialogAbortButtonText"),
                    Owner = this
                };
            saveDialog.ShowDialog();
        }

        /// <summary>
        /// Saves the current state to the specified profile name.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="SaveDialogEventArgs{T}"/> that contains the event data.</param>
        private void SaveNewProfileSuccess(object sender, SaveDialogEventArgs<string> e)
        {
            if (e != null && e.ActionType.Equals(SaveDialogActionType.Confirm) && string.IsNullOrEmpty(e.ReturnValue) == false)
            {
                this.ViewModel.SaveNewProfile(e.ReturnValue);
            }
        }
        #endregion

        #region overwrite specific sourceProfile

        /// <summary>Check if the <see cref="CommandBindingOverwriteSpecificProfileExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingOverwriteSpecificProfileCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            HostsFileProfile profile = e.Parameter as HostsFileProfile;
            if (this.ViewModel != null && profile != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        /// <summary>
        /// Save the current state to a specific HostsFile-Profile.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingOverwriteSpecificProfileExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            HostsFileProfile profile = e.Parameter as HostsFileProfile;
            if (this.ViewModel != null && profile != null)
            {
                this.ViewModel.SaveToProfile(profile);
            }
        }

        #endregion

        #region delete all profiles

        /// <summary>Check if the <see cref="CommandBindingDeleteAllProfilesExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingDeleteAllProfilesCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ViewModel != null;
        }

        /// <summary>
        /// Delete all profiles
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingDeleteAllProfilesExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.ViewModel != null)
            {
                this.ViewModel.DeleteAllProfiles();
            }
        }

        #endregion

        #region delete specific profile

        /// <summary>Check if the <see cref="CommandBindingDeleteSpecificProfileExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingDeleteSpecificProfileCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            HostsFileProfile profile = e.Parameter as HostsFileProfile;
            if (this.ViewModel != null && profile != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        /// <summary>
        /// Delete a specific HostsFile-Profile.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingDeleteSpecificProfileExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            HostsFileProfile profile = e.Parameter as HostsFileProfile;
            if (this.ViewModel != null && profile != null)
            {
                this.ViewModel.DeleteProfile(profile);
            }
        }

        #endregion

        #region close main window

        /// <summary>
        /// Close this <see cref="Application"/> instance.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="System.ComponentModel.CancelEventArgs"/> that contains the event data.</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SaveApplicationSettings();
        }

        #endregion

        #region exit

        /// <summary>Check if the <see cref="CommandBindingExitExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingExitCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Shuts the current <see cref="Application"/> instance down.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingExitExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.SaveApplicationSettings();
            Application.Current.Shutdown();
        }

        #endregion

        #region change language

        /// <summary>Check if the <see cref="CommandBindingChangeUiLanguageExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>        
        private void CommandBindingChangeUiLanguageCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Change the User-Interface language to the specified culture.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingChangeUiLanguageExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            /* get culture parameter */
            UiCultureInfo culture = e.Parameter as UiCultureInfo;
            if (culture == null)
            {
                return;
            }

            /* set ui culture */
            bool result = this.ViewModel.SetUiCulture(culture);
            if (!result)
            {
                return;
            }

            /* show dialog */
            string dialogTitle = App.GetApplicationResource<string>("cultureChangedDialogTitle");
            string dialogMessage = App.GetApplicationResource<string>("cultureChangedDialogMessage");
            string dialogConfirmButtonText = App.GetApplicationResource<string>("cultureChangedDialogConfirmButtonText");
            string dialogCancelButtonText = App.GetApplicationResource<string>("cultureChangedDialogCancelButtonText");
            Dialog cultureChangedDialogBox = new Dialog(this.CultureChangedDialogConfirmAction)
                {
                    Title = dialogTitle,
                    Message = dialogMessage,
                    ConfirmButtonText = dialogConfirmButtonText,
                    CancelButtonText = dialogCancelButtonText,
                    Owner = this
                };
            cultureChangedDialogBox.ShowDialog();
        }

        /// <summary>
        /// Saves the current state to the specified profile name.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="SaveDialogEventArgs{T}"/> that contains the event data.</param>
        private void CultureChangedDialogConfirmAction(object sender, DialogEventArgs e)
        {
            if (e == null || e.ActionType.Equals(DialogActionType.Confirm) == false)
            {
                return;
            }

            /* save changes */
            this.SaveApplicationSettings();

            /* add application restart event */
            Application.Current.Exit += (o, exitEventArgs) => System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);

            /* end application */
            Application.Current.Shutdown();
        }

        #endregion

        #region show about box

        /// <summary>Check if the <see cref="CommandBindingShowAboutBoxExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>        
        private void CommandBindingShowAboutBoxCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Show the "About-the-Application" window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingShowAboutBoxExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            About aboutBox = new About { Owner = this };
            aboutBox.ShowDialog();
        }

        #endregion

        #endregion

        #region user interface events

        #region load view: general

        /// <summary>Checks if the load-view functions can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>    
        private void CommandBindingSwitchViewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ViewModel != null;
        }

        #endregion

        #region load view: Overview

        /// <summary>
        /// Switches the <see cref="ViewMode"/> to <see cref="ViewMode.Overview"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingSwitchToOverviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.SwitchToView(ViewMode.Overview);
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
            this.ViewModel.SwitchToView(ViewMode.Editor);
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
            this.ViewModel.SwitchToView(ViewMode.TextEditor);
        }

        #endregion

        #region host file list selector

        /// <summary>Load the currently selected file from the file-list.</summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> event arguments.</param>
        private void FileListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null || comboBox.SelectedItem == null)
            {
                return;
            }

            string file = comboBox.SelectedItem as string;
            if (string.IsNullOrEmpty(file) == false)
            {
                this.ViewModel.LoadFile(file);
            }
        }

        #endregion

        #endregion

        #region private events

        /// <summary>
        /// Occurs when the data context for this element changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="DependencyPropertyChangedEventArgs"/> that contains the event data.</param> 
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.ApplyApplicationSettings();
        }

        #endregion

        #region private functions

        /// <summary>
        /// Save the current application-state (width, height, position, ...)
        /// </summary>
        private void SaveApplicationSettings()
        {
            ApplicationSettingsManager.Current.Settings.Width = Application.Current.MainWindow.Width;
            ApplicationSettingsManager.Current.Settings.Height = Application.Current.MainWindow.Height;
            ApplicationSettingsManager.Current.Settings.Top = Application.Current.MainWindow.Top;
            ApplicationSettingsManager.Current.Settings.Left = Application.Current.MainWindow.Left;
            ApplicationSettingsManager.Current.Settings.LastViewMode = this.ViewModel.InnerViewMode;
            ApplicationSettingsManager.Current.Settings.CultureCode = TranslationManager.Instance.CurrentCulture.ToString();
            ApplicationSettingsManager.Current.Settings.DisableWpfHardwareAcceleration = this.HardwareAccelerationIsDisabled;

            ApplicationSettingsManager.Current.SaveApplicationSettings();
        }

        /// <summary>Apply the specified <see cref="ApplicationSettings"/> to the current window.</summary>
        private void ApplyApplicationSettings()
        {
            if (ApplicationSettingsManager.Current.Settings != null)
            {
                this.Width = ApplicationSettingsManager.Current.Settings.Width;
                this.Height = ApplicationSettingsManager.Current.Settings.Height;
                this.Top = ApplicationSettingsManager.Current.Settings.Top;
                this.Left = ApplicationSettingsManager.Current.Settings.Left;
                this.HardwareAccelerationIsDisabled = ApplicationSettingsManager.Current.Settings.DisableWpfHardwareAcceleration;

                if (this.ViewModel != null)
                {
                    this.ViewModel.SwitchToView(ApplicationSettingsManager.Current.Settings.LastViewMode);
                }
            }

            return;
        }

        #endregion
    }
}
