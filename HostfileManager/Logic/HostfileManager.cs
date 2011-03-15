namespace HostfileManager.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;

    using DataAccess;
    using Model;
    using Model.Base;

    /// <summary>
    /// The <see cref="HostfileManager"/> class provides
    /// read and write access to host files and host file profiles.
    /// </summary>
    public class HostfileManager : IDisposable
    {
        #region private fields

        /// <summary>The key for the singleton accessor.</summary>
        private const string SingletonKey = "HostfileManager-Instance";

        /// <summary>The path the computers' hosts file.</summary>
        private string computerHostFilePath;

        /// <summary>An instance of the <see cref="HostfileDataAccess"/> data access layer class.</summary>
        private HostfileDataAccess dataAccess;

        #endregion

        #region constructor(s)

        /// <summary>Prevents a default instance of the <see cref="HostfileManager"/> class from being created.</summary>
        private HostfileManager()
        {
            /* attach event handlers */
            this.DataAccess.HostFileContentChanged += this.RaiseHostsFileContentChangedEvent;
            this.DataAccess.ProfilesChanged += this.RaiseProfilesChangedEvent;
        }

        #endregion

        #region singleton accessor

        /// <summary>Gets the <see cref="HostfileManager"/> class instance for the current application.</summary>
        public static HostfileManager Current
        {
            get
            {
                if (Application.Current.Properties.Contains(SingletonKey))
                {
                    return Application.Current.Properties[SingletonKey] as HostfileManager;
                }

                HostfileManager newInstance = new HostfileManager();
                Application.Current.Properties.Add(SingletonKey, newInstance);
                return newInstance;                
            }
        }

        #endregion

        #region public events

        /// <summary>
        /// Gets or sets the event that is fired whenever the contents of the hosts file have changed.
        /// </summary>
        public EventHandler<FileSystemEventArgs> HostFileContentChanged { get; set; }

        /// <summary>
        /// Gets or sets the event that is fired whenever a profile has been added or removed to the profile folder.
        /// </summary>
        public EventHandler ProfilesChanged { get; set; }

        #endregion

        #region public properties

        /// <summary>Gets the path of the main hosts file (default = "C:\Windows\System32\drivers\etc\hosts").</summary>
        public string ComputerHostFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(this.computerHostFilePath))
                {
                    this.computerHostFilePath = Constants.HostsFilePath;
                }

                return this.computerHostFilePath;
            }

            internal set
            {
                this.computerHostFilePath = value;
            }
        }

        #endregion

        #region protcted properties

        /// <summary>Gets an instance of the <see cref="HostfileDataAccess"/> data access layer class.</summary>
        protected HostfileDataAccess DataAccess
        {
            get
            {
                return this.dataAccess ?? (this.dataAccess = new HostfileDataAccess(this.ComputerHostFilePath, Constants.ProfileFileExtension, Constants.ProfileDirectory));
            }
        }

        #endregion

        #region public functions

        /// <summary>
        /// Get a <see cref="HostFile"/> object from the computers' hosts file.
        /// </summary>
        /// <returns>An initialized <see cref="HostFile"/> object.</returns>
        public HostFile GetHostsFile()
        {
            return this.GetHostsFile(this.ComputerHostFilePath);
        }

        /// <summary>
        /// Get a <see cref="HostFile"/> object from the file at the specified <paramref name="hostFilePath"/>.
        /// </summary>
        /// <param name="hostFilePath">The path to the hosts-file or an host file profile.</param>
        /// <returns>An initialized <see cref="HostFile"/> object; null if something went wrong.</returns>
        public HostFile GetHostsFile(string hostFilePath)
        {
            return this.GetHostfile(hostFilePath);
        }

        /// <summary>
        /// Return the <see cref="HostFile"/> object for the profile with the specified <paramref name="profilePath"/>.
        /// </summary>
        /// <param name="profilePath">The path to the hosts-file profile.</param>
        /// <returns>The <see cref="HostFile"/> object for the profile with the specified <paramref name="profilePath"/>.</returns>
        public HostFile GetProfile(string profilePath)
        {
            return this.GetHostsFile(profilePath);
        }

        /// <summary>Use the hosts file profile with the specified <paramref name="profilePath"/> and store it to the host file.</summary>
        /// <param name="profilePath">The profile path.</param>
        /// <returns>True if the profile under the specified <paramref name="profilePath"/> was successfully copied to the hosts file; otherwise false.</returns>
        public bool UseProfile(string profilePath)
        {
            return this.DataAccess.LoadProfile(profilePath);
        }

        /// <summary>Save the specified <paramref name="hostFile"/> to the specified <paramref name="targetPath"/>.</summary>
        /// <param name="hostFile">The <see cref="HostFile"/> object that shall be converted to a profile.</param>
        /// <param name="targetPath">The target path.</param>
        /// <returns>True if the profile was saved successfully; otherwise false.</returns>
        public bool SaveToProfile(HostFile hostFile, string targetPath)
        {
            if (hostFile == null)
            {
                throw new ArgumentNullException("hostFile");
            }

            if (string.IsNullOrEmpty(targetPath))
            {
                throw new ArgumentNullException("targetPath");
            }

            return this.DataAccess.SaveToProfile(this.ConvertHostfileToText(hostFile), targetPath);
        }

        /// <summary>
        /// Get a <see cref="List{HostsFileProfile}"/> of all <see cref="HostsFileProfile"/> objects stored in the <see cref="Constants.ProfileDirectory"/> of this application.
        /// </summary>
        /// <returns>A <see cref="List{HostsFileProfile}"/> of all <see cref="HostsFileProfile"/> objects.</returns>
        public List<HostsFileProfile> GetProfiles()
        {
            var profiles = this.DataAccess.GetProfilesInDirectory().Select(HostsFileProfile.CreateFromPath).Where(p => p != null);
            return profiles.ToList();
        }

        /// <summary>Get the full path of the current profile-directory.</summary>
        /// <returns>The full path of the current profile-directory (e.g. "C:\Programs\HostfileManager\").</returns>
        public string GetProfileDirectory()
        {
            return this.DataAccess.GetProfileDirectory();
        }

        /// <summary>
        /// Delete the specified <paramref name="profile"/> from the users hard-disk.
        /// </summary>
        /// <param name="profile">A <see cref="HostsFileProfile"/> object.</param>
        /// <returns>True if the specified <paramref name="profile"/> has been deleted from the users hard-disk; otherwise false.</returns>
        public bool DeleteProfile(HostsFileProfile profile)
        {
            return profile != null && this.DataAccess.DeleteProfile(profile.ProfilePath);
        }

        /// <summary>
        /// Save a <see cref="HostFile"/> object to its corresponding file.
        /// </summary>
        /// <param name="hostFile">A <see cref="HostFile"/> object instance.</param>
        /// <param name="filepath">The path to the file.</param>
        /// <returns>True if the <see cref="HostFile"/> object was successfully saved to its corresponding file on the users' hard drive (<see cref="HostFile.FilePath"/>). Otherwise false.</returns>
        public bool SaveHostsFileContent(HostFile hostFile, string filepath)
        {
            return this.DataAccess.SaveHostsFileContent(this.ConvertHostfileToText(hostFile), filepath);
        }

        /// <summary>
        /// Save the supplied <paramref name="content"/> to the file with the specified <paramref name="filepath"/>.
        /// </summary>
        /// <param name="content">The text content of a host file.</param>
        /// <param name="filepath">The target path.</param>
        /// <returns>True if the supplied <paramref name="content"/> was successfully saved to the file with the specified <paramref name="filepath"/>; oterwise false.</returns>
        public bool SaveHostsFileContent(string content, string filepath)
        {
            return this.DataAccess.SaveHostsFileContent(content, filepath);
        }

        /// <summary>Get the full profile path for the profile with the specified <paramref name="profileName"/>.</summary>
        /// <param name="profileName">The profile name.</param>
        /// <returns>The full profile path for the specified  <paramref name="profileName"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="profileName"/> cannot be null.</exception>
        public string GetProfilePath(string profileName)
        {
            if (string.IsNullOrEmpty(profileName))
            {
                throw new ArgumentNullException("profileName");
            }

            string path = Path.Combine(Constants.ProfileDirectory, string.Concat(profileName, Constants.ProfileFileExtension));
            return path;
        }

        /// <summary>
        /// Convert the specified <see cref="HostFile"/> object to text.
        /// </summary>
        /// <param name="file">A <see cref="HostFile"/> object instance.</param>
        /// <returns>The text-representation of the specified <paramref name="file"/>.</returns>
        public string ConvertHostfileToText(HostFile file)
        {
            return file != null ? file.GetText() : string.Empty;
        }

        /// <summary>Restores the default hosts-file content.</summary>
        /// <returns>True if the default hosts-file could be restored; otherwise false.</returns>
        public bool RestoreDefaultHostsFile()
        {
            string defaultHostsFileContent = App.GetApplicationResource("Hostfile_DefaultText", string.Empty);
            return this.SaveHostsFileContent(defaultHostsFileContent, this.ComputerHostFilePath);
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.DataAccess.Dispose();
        }

        #endregion

        #region private functions

        /// <summary>Get a <see cref="HostFile"/> object for the host file with the specified <paramref name="hostFilePath"/>.</summary>
        /// <param name="hostFilePath">The host file path.</param>
        /// <returns>An initialized <see cref="HostFile"/> object.</returns>
        private HostFile GetHostfile(string hostFilePath)
        {
            List<string> lines = this.DataAccess.GetHostfileContent(hostFilePath);

            HostFile hostFile = HostFile.CreateHostFile(hostFilePath);
            HostfileLineByLineNavigator navigator = new HostfileLineByLineNavigator(lines);
            HostfileEntryCollection collection = new HostfileEntryCollection(hostFile.PropertyChangedCallBack);

            foreach (HostfileLine line in navigator)
            {
                if (line.IsMultiLineComment)
                {
                    /* multi line comment */
                    if (collection.GetElementBy(HostfileEntryType.Comment, line.MultiLineCommentText) == null)
                    {
                        collection.Add(new Comment(line.MultiLineCommentText, hostFile.PropertyChangedCallBack));
                    }
                }
                else if (line.IsToggleModeOption)
                {
                    string value = line.Values.FirstOrDefault();

                    if (value != null)
                    {
                        /* group toggle-mode */
                        if (value.Equals("group", StringComparison.OrdinalIgnoreCase))
                        {
                            hostFile.ExclusiveGroupToggleModeIsEnabled = true;
                        }

                        /* host toggle-mode */
                        if (value.Equals("host", StringComparison.OrdinalIgnoreCase))
                        {
                            hostFile.ExclusiveHostToggleModeIsEnabled = true;
                        }
                    }
                }
                else if (line.IsGlobalComment)
                {
                    /* single comment */
                    collection.Add(new Comment(line.Values.FirstOrDefault(), hostFile.PropertyChangedCallBack));
                }
                else if (line.IsGroupStart)
                {
                    /* host group */
                    HostGroup group = new HostGroup(line.GroupName, line.DescriptionText, hostFile.PropertyChangedCallBack);
                    collection.Add(group);
                }
                else if (line.IsHost)
                {
                    /* host */
                    IHostfileEntry parentItem = line.IsMemberOfHostGroup
                                                     ? collection.GetElementBy(
                                                         HostfileEntryType.HostGroup, line.GroupName)
                                                     : null;
                    HostGroup parentGroup = parentItem != null ? parentItem as HostGroup : null;
                    Host host = new Host(parentGroup, line.Values.FirstOrDefault(), line.DescriptionText, hostFile.PropertyChangedCallBack) { IsActive = line.IsActive };

                    /* domains */
                    string domainString = line.Values.LastOrDefault();
                    if (string.IsNullOrEmpty(domainString) == false)
                    {
                        List<string> domainNames = domainString.Split(' ').Select(domain => domain.Trim()).ToList();
                        foreach (string domainName in domainNames)
                        {
                            host.Childs.Add(new Domain(host, domainName, hostFile.PropertyChangedCallBack));
                        }
                    }

                    if (parentGroup != null)
                    {
                        parentGroup.Childs.Add(host);
                    }
                    else
                    {
                        collection.Add(host);
                    }
                }
            }

            /* attach entry collection to host file */
            hostFile.Childs = collection;

            return hostFile;
        }

        #endregion

        #region private event handlers

        /// <summary>
        /// Raise the <see cref="HostFileContentChanged"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="FileSystemEventArgs"/> that contains the event data.</param>    
        private void RaiseHostsFileContentChangedEvent(object sender, FileSystemEventArgs e)
        {
            if (this.HostFileContentChanged != null)
            {
                this.HostFileContentChanged(sender, e);
            }
        }

        /// <summary>
        /// Raise the <see cref="ProfilesChanged"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="FileSystemEventArgs"/> that contains the event data.</param>    
        private void RaiseProfilesChangedEvent(object sender, EventArgs e)
        {
            if (this.ProfilesChanged != null)
            {
                this.ProfilesChanged(sender, e);
            }
        }

        #endregion
    }
}
