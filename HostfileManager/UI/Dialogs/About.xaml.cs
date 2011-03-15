namespace HostfileManager.UI.Dialogs
{
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Navigation;

    /// <summary>
    /// The <see cref="About"/> class displays the about-this-application window to the user.
    /// </summary>
    public partial class About
    {
        #region public fields / ui commands

        /// <summary>
        /// The Application-Exit command.
        /// </summary>
        public static readonly RoutedCommand ExitCommand = new RoutedCommand();

        #endregion

        #region constructor(s)

        /// <summary>Initializes a new instance of the <see cref="About"/> class.</summary>
        public About()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        #endregion

        #region Assembly Attribute Accessors

        /// <summary>Gets current applications's assembly version.</summary>
        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        #endregion

        #region user interface events

        /// <summary>Handle a hyperlink click event</summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The <see cref="RequestNavigateEventArgs"/> object.</param>
        private void HyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Hyperlink hl = (Hyperlink)sender;
            string navigateUri = hl.NavigateUri.ToString();
            Process.Start(new ProcessStartInfo(navigateUri));
            e.Handled = true;
        }

        #region exit

        /// <summary>Check if the <see cref="CommandBindingExitExecuted"/> function can be executed in the current context.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingExitCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Close this window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void CommandBindingExitExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #endregion
    }
}
