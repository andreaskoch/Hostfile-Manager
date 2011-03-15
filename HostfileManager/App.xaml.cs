namespace HostfileManager
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Shell;

    using Logic;
    using Model;
    using Model.Base;
    using UI;
    using ViewModel;

    /// <summary>
    /// The <see cref="App"/> class starts the HostfileManager application.
    /// </summary>
    public partial class App
    {
        #region private members

        /// <summary>A flag indicating whether the user-interface culture has been set.</summary>
        private bool userInterfaceCultureHasBeenSet;

        /// <summary>The application directory path of the current application instance.</summary>
        private string applicationDirectory;

        /// <summary>The application path of the current application instance.</summary>
        private string applicationPath;

        /// <summary>An instance of the <see cref="MainWindowViewModel"/> of this application.</summary>
        private MainWindowViewModel appViewModel;

        /// <summary>An instance of the <see cref="MainWindow"/> object of this application.</summary>
        private MainWindow appWindow;

        #endregion

        #region public properties

        /// <summary>
        /// Gets a value indicating whether this <see cref="App"/> is in console-only mode or not (default = false).
        /// </summary>
        public bool ConsoleOnly { get; private set; }

        /// <summary>Gets an instance of the <see cref="MainWindowViewModel"/> of this application.</summary>
        public MainWindowViewModel AppViewModel
        {
            get
            {
                return this.appViewModel ?? (this.appViewModel = new MainWindowViewModel());
            }
        }

        /// <summary>Gets an instance of the <see cref="MainWindow"/> object of this application.</summary>
        public MainWindow AppWindow
        {
            get
            {
                return this.appWindow ?? (this.appWindow = new MainWindow { DataContext = this.AppViewModel });
            }
        }

        /// <summary>Gets the application directory path of the current application instance (e.g. "C:\\Users\\Administrator\\Desktop\\bin\\HostFileManager\\bin\\Release").</summary>
        public string ApplicationDirectory
        {
            get
            {
                return this.applicationDirectory ?? (this.applicationDirectory = AppDomain.CurrentDomain.BaseDirectory);
            }
        }

        /// <summary>Gets the application path of the current application instance (e.g. "file:///C:/Users/Administrator/Desktop/bin/HostFileManager/bin/Release/HostfileManager.exe").</summary>
        public string ApplicationPath
        {
            get
            {
                return this.applicationPath ?? (this.applicationPath = string.Concat(this.ApplicationDirectory, AppDomain.CurrentDomain.FriendlyName));
            }
        }

        #endregion

        #region public static functions

        /// <summary>Get a specific application string from the application resources.</summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="resourceName">The name of the application-resource.</param>
        /// <param name="defaultValue">The default Value.</param>
        /// <returns>The string value stored for the specified <paramref name="resourceName"/> if it does exist; otherwise null.</returns>
        public static T GetApplicationResource<T>(string resourceName, T defaultValue = default(T))
        {
            if (Current.Resources.Contains(resourceName))
            {
                try
                {
                    return (T)Current.Resources[resourceName];
                }
                catch (InvalidCastException)
                {
                }
            }

            if (typeof(T).Equals(typeof(string)))
            {
                return (T)((object)resourceName);
            }

            return defaultValue;
        }

        #endregion

        #region application lifecycle

        /// <summary>Occurs when the Run method of the <see cref="Application"/> object is called.</summary>
        /// <param name="e">A <see cref="StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            /* parse language agnostic application parameters */
            this.EvaluateLanguageAgnosticCommandLineArguments(e);

            /* apply user-interface language from app settings */
            if (this.userInterfaceCultureHasBeenSet == false && ApplicationSettingsManager.Current.Settings.CultureCode != null)
            {
                TranslationManager.Instance.SetUserInterfaceLanguage(ApplicationSettingsManager.Current.Settings.CultureCode);
            }

            /* parse language specific application parameters */
            EvaluateLanguageDependentCommandLineArguments(e);

            /* configure jump list */
            this.InitJumpList();

            if (this.ConsoleOnly == false)
            {
                /* initialize UI */
                this.AppWindow.Show();
            }
            else
            {
                /* end the application */
                this.Shutdown();
            }
        }

        #endregion

        #region private static functions

        /// <summary>
        /// Evaluate language dependent command line arguments.
        /// </summary>
        /// <param name="e">A <see cref="StartupEventArgs"/> that contains the event data.</param>
        private static void EvaluateLanguageDependentCommandLineArguments(StartupEventArgs e)
        {
            if (e == null || e.Args.Length <= 0)
            {
                return;
            }

            /* restore default hosts file */
            string cmdArgRestoreDefaultHostsFileContent = string.Concat(Constants.CommandLineArgumentPrefix, Constants.CommandLineArgumentRestoreDefaultHostsFileContent);
            string restoreDefaultHostsFileContentArg = e.Args.FirstOrDefault(arg => arg.Equals(cmdArgRestoreDefaultHostsFileContent, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(restoreDefaultHostsFileContentArg) == false)
            {
                HostfileManager.Current.RestoreDefaultHostsFile();
            }
        }

        #endregion

        #region private methods

        /// <summary>
        /// Initialize the application's jump list.
        /// </summary>
        private void InitJumpList()
        {
            JumpList jumpList = new JumpList { ShowFrequentCategory = false, ShowRecentCategory = false };

            /* jumplist reset application settings */
            JumpTask jumpListTaskResetAppSettings = new JumpTask
                {
                    Title = GetApplicationResource<string>("jumplistActionTitleResetApplicationSettings"),
                    Arguments =
                        string.Concat(
                            Constants.CommandLineArgumentPrefix,
                            Constants.CommandLineArgumentResetApplicationSettings,
                            " ",
                            Constants.CommandLineArgumentPrefix,
                            Constants.CommandLineArgumentConsoleOnly),
                    Description = GetApplicationResource<string>("jumplistActionDescriptionResetApplicationSettings"),
                    CustomCategory = GetApplicationResource<string>("jumplistGroupActions"),
                    WorkingDirectory = this.ApplicationDirectory,
                    IconResourcePath = this.ApplicationPath,
                    IconResourceIndex = -1,
                    ApplicationPath = this.ApplicationPath
                };
            jumpList.JumpItems.Add(jumpListTaskResetAppSettings);

            /* jumplist - open profile directory */
            JumpTask jumpListTaskOpenProfileDirectory = new JumpTask
                {
                    Title = GetApplicationResource<string>("jumplistActionTitleOpenProfileDirectory"),
                    Arguments =
                        string.Concat(
                            Constants.CommandLineArgumentPrefix,
                            Constants.CommandLineArgumentOpenProfileDirectory,
                            " ",
                            Constants.CommandLineArgumentPrefix,
                            Constants.CommandLineArgumentConsoleOnly),
                    Description = GetApplicationResource<string>("jumplistActionDescriptionOpenProfileDirectory"),
                    CustomCategory = GetApplicationResource<string>("jumplistGroupActions"),
                    WorkingDirectory = this.ApplicationDirectory,
                    IconResourcePath = this.ApplicationPath,
                    IconResourceIndex = -1,
                    ApplicationPath = this.ApplicationPath
                };
            jumpList.JumpItems.Add(jumpListTaskOpenProfileDirectory);

            /* jumplist - load profile */
            foreach (HostsFileProfile p in HostfileManager.Current.GetProfiles())
            {
                JumpTask task = new JumpTask
                    {
                        Title =
                            string.Format(
                                GetApplicationResource<string>("jumplistActionTitleLoadProfile"), p.ProfileName),
                        Arguments =
                            string.Concat(
                                "\"",
                                Constants.CommandLineArgumentPrefix,
                                Constants.CommandLineArgumentLoadProfile,
                                "=",
                                p.ProfilePath,
                                "\""),
                        Description =
                            string.Format(
                                GetApplicationResource<string>("jumplistActionDescriptionLoadProfile"), p.ProfileName),
                        CustomCategory = GetApplicationResource<string>("jumplistGroupLoadProfile"),
                        WorkingDirectory = this.ApplicationDirectory,
                        IconResourcePath = this.ApplicationPath,
                        ApplicationPath = this.ApplicationPath
                    };
                jumpList.JumpItems.Add(task);
            }

            JumpList.SetJumpList(Current, jumpList);
        }

        /// <summary>
        /// Evaluate language agnostic command line arguments.
        /// </summary>
        /// <param name="e">A <see cref="StartupEventArgs"/> that contains the event data.</param>
        private void EvaluateLanguageAgnosticCommandLineArguments(StartupEventArgs e)
        {
            if (e == null || e.Args.Length <= 0)
            {
                return;
            }

            /* set user-interface culture */
            Regex regExUserInterfaceCulture = new Regex(string.Concat(Constants.CommandLineArgumentPrefix, Constants.CommandLineArgumentPatternSetUserInterfaceCulture), RegexOptions.IgnoreCase);
            string cultureArg = e.Args.FirstOrDefault(regExUserInterfaceCulture.IsMatch);
            if (string.IsNullOrEmpty(cultureArg) == false)
            {
                MatchCollection matches = regExUserInterfaceCulture.Matches(cultureArg);
                if (matches.Count > 0)
                {
                    Match match = matches[0];
                    if (match.Groups.Count > 0)
                    {
                        Group g = match.Groups[match.Groups.Count - 1];
                        string cultureCode = g.Value;

                        /* try to set the ui culture */
                        if (TranslationManager.Instance.SetUserInterfaceLanguage(cultureCode))
                        {
                            this.userInterfaceCultureHasBeenSet = true;
                        }
                    }
                }
            }

            /* load profile */
            Regex regExProfilePath = new Regex(string.Concat(Constants.CommandLineArgumentPrefix, Constants.CommandLineArgumentPatternLoadProfile), RegexOptions.IgnoreCase);
            string profilePathArg = e.Args.FirstOrDefault(regExProfilePath.IsMatch);
            if (string.IsNullOrEmpty(profilePathArg) == false)
            {
                MatchCollection matches = regExProfilePath.Matches(profilePathArg);
                if (matches.Count > 0)
                {
                    Match match = matches[0];
                    if (match.Groups.Count > 0)
                    {
                        Group g = match.Groups[match.Groups.Count - 1];
                        string profilePath = g.Value;

                        /* copy profile to hosts file */
                        HostfileManager.Current.UseProfile(profilePath);
                    }
                }
            }

            /* set a new hosts file path */
            Regex regExHostsFilePath = new Regex(string.Concat(Constants.CommandLineArgumentPrefix, Constants.CommandLineArgumentPatternHostsFilePath), RegexOptions.IgnoreCase);
            string hostsFilePathArg = e.Args.FirstOrDefault(regExHostsFilePath.IsMatch);
            if (string.IsNullOrEmpty(hostsFilePathArg) == false)
            {
                MatchCollection matches = regExHostsFilePath.Matches(hostsFilePathArg);
                if (matches.Count > 0)
                {
                    Match match = matches[0];
                    if (match.Groups.Count > 0)
                    {
                        Group g = match.Groups[match.Groups.Count - 1];
                        string newHostsFilePath = g.Value;
                        HostfileManager.Current.ComputerHostFilePath = newHostsFilePath;
                    }
                }
            }

            /* set group status */
            Regex regExSetGroupStatus = new Regex(string.Concat(Constants.CommandLineArgumentPrefix, Constants.CommandLineArgumentPatternSetGroupStatus), RegexOptions.IgnoreCase);
            string groupStatusArg = e.Args.FirstOrDefault(regExSetGroupStatus.IsMatch);
            if (string.IsNullOrEmpty(groupStatusArg) == false)
            {
                MatchCollection matches = regExSetGroupStatus.Matches(groupStatusArg);
                if (matches.Count > 0)
                {
                    Match match = matches[0];
                    if (match.Groups.Count > 2)
                    {
                        Group actionGroup = match.Groups[match.Groups.Count - 2];
                        Group modifierGroup = match.Groups[match.Groups.Count - 1];

                        if (actionGroup.Success && modifierGroup.Success)
                        {
                            bool newStatus = actionGroup.Value.Equals(Constants.CommandLineArgumentComponentGroupStatusEnable, StringComparison.OrdinalIgnoreCase) ? true : false;
                            string modifierValue = modifierGroup.Value;

                            /* set new group status */
                            HostFile hf = HostfileManager.Current.GetHostsFile();
                            foreach (HostGroup group in hf.Childs.Where(group => group.EntryType.Equals(HostfileEntryType.HostGroup) && (group.Name.Equals(modifierValue, StringComparison.OrdinalIgnoreCase) || modifierValue.Equals("all", StringComparison.OrdinalIgnoreCase))).OfType<HostGroup>().Where(group => group != null))
                            {
                                group.IsActive = newStatus;
                            }

                            /* save changes */
                            HostfileManager.Current.SaveHostsFileContent(hf, hf.FilePath);
                        }
                    }
                }
            }

            /* disable hardware acceleration */
            string cmdArgDisableHardwareAcceleration = string.Concat(Constants.CommandLineArgumentPrefix, Constants.CommandLineArgumentDisableHardwareAcceleration);
            string disableHardwareAccelerationArg = e.Args.FirstOrDefault(arg => arg.Equals(cmdArgDisableHardwareAcceleration, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(disableHardwareAccelerationArg) == false)
            {
                ApplicationSettingsManager.Current.Settings.DisableWpfHardwareAcceleration = true;
            }

            /* open profile directory */
            string cmdArgOpenProfileDirectory = string.Concat(Constants.CommandLineArgumentPrefix, Constants.CommandLineArgumentOpenProfileDirectory);
            string openProfileDirectoryArg = e.Args.FirstOrDefault(arg => arg.Equals(cmdArgOpenProfileDirectory, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(openProfileDirectoryArg) == false)
            {
                string profileDirectoryPath = HostfileManager.Current.GetProfileDirectory();
                if (string.IsNullOrEmpty(profileDirectoryPath) == false)
                {
                    Process.Start(new ProcessStartInfo(profileDirectoryPath));
                }
            }

            /* console only */
            string cmdArgConsoleOnly = string.Concat(Constants.CommandLineArgumentPrefix, Constants.CommandLineArgumentConsoleOnly);
            string consoleOnlyArg = e.Args.FirstOrDefault(arg => arg.Equals(cmdArgConsoleOnly, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(consoleOnlyArg) == false)
            {
                this.ConsoleOnly = true;
            }

            /* reset application settings */
            string cmdArgReset = string.Concat(Constants.CommandLineArgumentPrefix, Constants.CommandLineArgumentResetApplicationSettings);

            string resetArg = e.Args.FirstOrDefault(arg => arg.Equals(cmdArgReset, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(resetArg) == false)
            {
                ApplicationSettingsManager.Current.ResetApplicationSettings();
            }
        }

        #endregion
    }
}
