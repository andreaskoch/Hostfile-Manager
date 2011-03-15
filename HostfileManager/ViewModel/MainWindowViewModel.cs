namespace HostfileManager.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using HostfileManager.Logic;
    using HostfileManager.Model;
    using HostfileManager.UI;

    /// <summary>
    /// The ViewModel class for the <see cref="MainWindow"/> user-interface.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region private members

        /// <summary>
        /// The current ViewModel for the <see cref="MainWindow"/>.
        /// </summary>
        private readonly HostsFileViewModel innerViewModel;

        /// <summary>
        /// The current UI View-Mode.
        /// </summary>
        private ViewMode innerViewMode;

        /// <summary>
        /// A flag indicating whether this object has been disposed.
        /// </summary>
        private bool disposed;

        #endregion

        #region constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            /* attach profiles-changed event */
            HostfileManager.Current.ProfilesChanged += this.ProfilesChanged;

            /* load host file view-model */
            this.innerViewModel = new HostsFileViewModel();

            /* attach event listeners */
            this.InnerViewModel.HostsFileObjectChanged += this.InnerViewModelHostsFileObjectChanged;
        }

        #endregion

        #region public read-only properties

        /// <summary>
        /// Gets or sets the current <see cref="ViewMode"/> of the user-interface.
        /// </summary>
        public ViewMode InnerViewMode
        {
            get
            {
                return this.innerViewMode;
            }

            protected set
            {
                this.innerViewMode = value;

                this.OnPropertyChanged("InnerViewMode");

                /* calculated properties */
                this.OnPropertyChanged("GroupOverviewIsVisible");
                this.OnPropertyChanged("GroupEditorIsVisible");
                this.OnPropertyChanged("TextEditorIsVisible");
            }
        }

        /// <summary>
        /// Gets the current inner ViewModel for the <see cref="MainWindow"/>.
        /// </summary>
        public HostsFileViewModel InnerViewModel
        {
            get
            {
                return this.innerViewModel;
            }
        }

        /// <summary>Gets the current user-interface culture (e.g. "en-GB").</summary>
        public CultureInfo CurrentCulture
        {
            get
            {
                return TranslationManager.Instance.CurrentCulture;
            }
        }

        /// <summary>Gets a list of all supported user-interface languages.</summary>
        public List<UiCultureInfo> Languages
        {
            get
            {
                return TranslationManager.Instance.Languages.ConvertAll(culture => new UiCultureInfo(culture));
            }
        }

        /// <summary>
        /// Gets a <see cref="List{T}"/> of <see cref="HostsFileProfile"/> objects.
        /// </summary>
        public List<HostsFileProfile> Profiles
        {
            get
            {
                return HostfileManager.Current.GetProfiles();
            }
        }

        /// <summary>
        /// Gets a value indicating whether there are sourceProfile available or not.
        /// </summary>
        public bool HasProfiles
        {
            get
            {
                return this.Profiles.Count > 0;
            }
        }

        /// <summary>Gets the file path of the current <see cref="HostFile"/> object.</summary>
        public string TargetFilePath
        {
            get
            {
                if (this.InnerViewModel != null && this.InnerViewModel.HostFileInstance != null)
                {
                    return this.InnerViewModel.HostFileInstance.FilePath;
                }

                return string.Empty;
            }
        }

        /// <summary>Gets a list of target files.</summary>
        public List<string> TargetFiles
        {
            get
            {
                List<string> files = new List<string> { HostfileManager.Current.ComputerHostFilePath };
                files.AddRange(this.Profiles.Select(p => p.ProfilePath));

                files = files.Distinct().ToList();

                return files;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ViewMode.Overview"/> view-mode is active or not.
        /// </summary>
        public bool GroupOverviewIsVisible
        {
            get
            {
                return this.InnerViewMode == ViewMode.Overview;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ViewMode.Editor"/> view-mode is active or not.
        /// </summary>
        public bool GroupEditorIsVisible
        {
            get
            {
                return this.InnerViewMode == ViewMode.Editor;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ViewMode.TextEditor"/> view-mode is active or not.
        /// </summary>
        public bool TextEditorIsVisible
        {
            get
            {
                return this.InnerViewMode == ViewMode.TextEditor;
            }
        }

        #endregion

        #region public functions

        /// <summary>
        /// Discard all changes and reload the current hosts file from disk.
        /// </summary>
        public void ReloadFromDisk()
        {
            this.InnerViewModel.ReloadFromDisk(this.InnerViewMode);
        }

        /// <summary>
        /// Copy the text-representation of the current <see cref="HostFile"/> to the user's clipboard.
        /// </summary>
        public void CopyToClipboard()
        {
            this.InnerViewModel.CopyToClipboard(this.InnerViewMode);
        }

        /// <summary>
        /// Loads the specified <see cref="HostsFileProfile"/>.
        /// </summary>
        /// <param name="sourceProfile">A <see cref="HostsFileProfile"/> object.</param>
        public void LoadProfile(HostsFileProfile sourceProfile)
        {
            if (sourceProfile == null)
            {
                throw new ArgumentNullException("sourceProfile");
            }

            if (this.InnerViewModel != null)
            {
                this.innerViewModel.LoadProfile(sourceProfile);
            }
        }

        /// <summary>Loads the hosts file with specified <paramref name="filePath"/>.</summary>
        /// <param name="filePath">The file path to a hosts file or a hosts file profile.</param>
        public void LoadFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath");
            }

            if (this.InnerViewModel != null)
            {
                this.InnerViewModel.LoadFile(filePath);
            }
        }

        /// <summary>Save all changes to the current computers hosts file.</summary>
        public void SaveChangesToHostFile()
        {
            this.InnerViewModel.SaveChanges(this.InnerViewMode, HostfileManager.Current.ComputerHostFilePath);
        }

        /// <summary>Save all changes made to the currently loaded hosts-file.</summary>
        public void SaveChangesToCurrentFile()
        {
            this.InnerViewModel.SaveChanges(this.InnerViewMode, this.TargetFilePath);
        }

        /// <summary>
        /// Saves the current application state to a a new profile.
        /// </summary>
        /// <param name="profileName">The name of the new hosts-file profile.</param>
        /// <returns>True if the profile has been saved sucessfully; otherwise false.</returns>
        public bool SaveNewProfile(string profileName)
        {
            if (string.IsNullOrEmpty(profileName))
            {
                throw new ArgumentNullException("profileName");
            }

            if (this.InnerViewModel != null)
            {
                if (this.InnerViewModel.HostFileInstance != null)
                {
                    string profilePath = HostfileManager.Current.GetProfilePath(profileName);
                    return HostfileManager.Current.SaveToProfile(this.InnerViewModel.HostFileInstance, profilePath);
                }
            }

            return false;
        }

        /// <summary>
        /// Saves the current application state to the specified <paramref name="targetProfile"/> object.
        /// </summary>
        /// <param name="targetProfile">A <paramref name="targetProfile"/> object.</param>
        /// <returns>True if the current application state was successfully saved to the specified <paramref name="targetProfile"/>; Otherwise false.</returns>
        public bool SaveToProfile(HostsFileProfile targetProfile)
        {
            if (targetProfile == null)
            {
                throw new ArgumentNullException("targetProfile");
            }

            if (this.InnerViewModel != null)
            {
                if (this.InnerViewModel.HostFileInstance != null)
                {
                    return HostfileManager.Current.SaveToProfile(this.InnerViewModel.HostFileInstance, targetProfile.ProfilePath);
                }
            }

            return false;
        }

        /// <summary>
        /// Delete all profiles from the current users hard-disk.
        /// </summary>
        public void DeleteAllProfiles()
        {
            List<HostsFileProfile> profiles = this.Profiles;
            foreach (HostsFileProfile p in profiles)
            {
                this.DeleteProfile(p);
            }
        }

        /// <summary>
        /// Delete the specified <paramref name="targetProfile"/> from the user's hard-disk.
        /// </summary>
        /// <param name="targetProfile">A <paramref name="targetProfile"/> object.</param>
        /// <returns>True if the specified <paramref name="targetProfile"/> has been successfully deleted; otherwise false.</returns>
        public bool DeleteProfile(HostsFileProfile targetProfile)
        {
            return HostfileManager.Current.DeleteProfile(targetProfile);
        }

        /// <summary>Change the User-Interface language to the specified culture.</summary>
        /// <param name="culture">The culture Info.</param>
        /// <returns>True if the UI culture has been set; otherwise false.</returns>
        public bool SetUiCulture(UiCultureInfo culture)
        {
            bool result = TranslationManager.Instance.SetUserInterfaceLanguage(culture);
            return result;
        }

        /// <summary>Switch the view to the specified <paramref name="viewMode"/>.</summary>
        /// <param name="viewMode">The view mode.</param>
        public void SwitchToView(ViewMode viewMode)
        {
            this.InnerViewMode = viewMode;
        }

        #endregion

        #region IDisposable members

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// <param name="disposing">A flag indicating whether this object is currently disposing.</param>
        protected override void Dispose(bool disposing)
        {
            if (this.disposed == false)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                    this.innerViewModel.Dispose();
                }

                // Free your own state (unmanaged objects).
                // Set large fields to null.
                this.disposed = true;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region private function, methods and events

        /// <summary>Event handler that is raised whenever the underlying <see cref="HostFile"/> object of the <see cref="InnerViewModel"/> changed.</summary>
        /// <param name="sender">The sender <see cref="object"/>.</param>
        /// <param name="e">The <see cref="EventArgs"/> event parameter object.</param>
        private void InnerViewModelHostsFileObjectChanged(object sender, EventArgs e)
        {
            this.OnPropertyChanged("TargetFilePath");
        }

        /// <summary>
        /// Updates the user-interface with the new list of profiles.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>    
        private void ProfilesChanged(object sender, EventArgs e)
        {
            /* update ui */
            OnPropertyChanged("Profiles");
            OnPropertyChanged("HasProfiles");
            OnPropertyChanged("TargetFiles");
        }

        #endregion
    }
}
