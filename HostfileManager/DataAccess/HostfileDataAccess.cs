namespace HostfileManager.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Security.Permissions;

    using HostfileManager.Logic;

    /// <summary>
    /// The <see cref="HostfileDataAccess"/> class provides file-level
    /// access to the users' hosts file and host file profiles.
    /// </summary>
    public class HostfileDataAccess : IDisposable
    {
        #region private members

        /// <summary>
        /// The path to the hosts file (e.g. "C:\Windows\System32\drivers\etc\hosts").
        /// </summary>
        private readonly string hostsFilePath;

        /// <summary>The file extension for host file profiles (e.g. ".hostfileprofile").</summary>
        private readonly string profileFileExtension;

        /// <summary>The path to the directory in which all host file profiles are stored.</summary>
        private readonly string profileDirectory;

        /// <summary>
        /// A flag indicating wehther this object has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// A <see cref="FileSystemWatcher"/> object for watching changes on the hosts-file.
        /// </summary>
        private FileSystemWatcher hostsFileWatcher;

        /// <summary>
        /// A <see cref="FileSystemWatcher"/> object for watching changes to the profiles folder.
        /// </summary>
        private FileSystemWatcher profilesWatcher;

        #endregion

        #region constructor(s) and destructor(s)

        /// <summary>Initializes a new instance of the <see cref="HostfileDataAccess"/> class.</summary>
        /// <param name="computerHostFilePath">The path to the hosts file (usually this should be "C:\Windows\System32\drivers\etc\hosts").</param>
        /// <param name="profileFileExtension">The profile file extension (e.g. ".hostfileprofile").</param>
        /// <param name="profileDirectory">The profile directory path (e.g. ".\" ==> current directory).</param>
        public HostfileDataAccess(string computerHostFilePath, string profileFileExtension, string profileDirectory)
        {
            this.hostsFilePath = computerHostFilePath;
            this.profileFileExtension = profileFileExtension;
            this.profileDirectory = profileDirectory;

            /* attach FileSystemWatcher to hosts file */
            this.InitHostsFileMonitor();

            /* attach FileSystemWatcher to profiles */
            this.InitProfilesMonitor();
        }

        /// <summary>Finalizes an instance of the <see cref="HostfileDataAccess"/> class. </summary>
        ~HostfileDataAccess()
        {
            this.Dispose(false);
        }

        #endregion

        #region public events

        /// <summary>
        /// Gets or sets the event that is fired whenever the contents of the hosts file have changed.
        /// </summary>
        public EventHandler<FileSystemEventArgs> HostFileContentChanged { get; set; }

        /// <summary>
        /// Gets or sets the event that is fired whenever a profile has been added to or removed from the profile folder (<see cref="profileDirectory"/>).
        /// </summary>
        public EventHandler ProfilesChanged { get; set; }

        #endregion

        #region public functions

        /// <summary>Get the content of hosts file (or host file profile) with the specified <paramref name="filePath"/>.</summary>
        /// <param name="filePath">The path to the hosts file or an host file profile.</param>
        /// <returns>All lines from the hostfile with the specified <paramref name="filePath"/>.</returns>
        public List<string> GetHostfileContent(string filePath)
        {
            this.VerifyFile(filePath);

            using (var stream = new StreamReader(filePath))
            {
                return (from line in stream.Lines() select line).ToList();
            }
        }

        /// <summary>Use the hosts file profile with the specified <paramref name="profilePath"/> and store it to the host file.</summary>
        /// <param name="profilePath">The profile path.</param>
        /// <returns>True if the profile under the specified <paramref name="profilePath"/> was successfully copied to the hosts file; otherwise false.</returns>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public bool LoadProfile(string profilePath)
        {
            return this.IsHostFileProfile(profilePath) && CopyFile(profilePath, this.hostsFilePath);
        }

        /// <summary>Save the supplied <paramref name="text"/> to the specified <paramref name="targetPath"/>.</summary>
        /// <param name="text">The text content of an host file or host file profile.</param>
        /// <param name="targetPath">The target path.</param>
        /// <returns>True if the profile was saved successfully; otherwise false.</returns>
        public bool SaveToProfile(string text, string targetPath)
        {
            if (text != null && string.IsNullOrEmpty(targetPath) == false && this.IsHostFileProfile(targetPath))
            {
                return this.WriteFileContent(targetPath, text);
            }

            return false;
        }

        /// <summary>
        /// Save the specified <paramref name="content"/> back to the current user's hosts file.
        /// </summary>
        /// <param name="content">The new hosts file content.</param>
        /// <param name="filepath">The path to the file.</param>
        /// <returns>True if the specified <paramref name="content"/> was successfully saved; otherwise false.</returns>
        public bool SaveHostsFileContent(string content, string filepath)
        {
            this.VerifyFile(filepath);

            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            return this.WriteFileContent(filepath, content);
        }

        /// <summary>Return all profiles in the specified directory.</summary>
        /// <returns>An <see cref="IEnumerable{String}"/> of file paths to the profiles stored in the <see cref="profileDirectory"/>.</returns>
        public IEnumerable<string> GetProfilesInDirectory()
        {
            return from f in GetFilesInDirectory(this.profileDirectory) where f.Extension.Equals(this.profileFileExtension, StringComparison.OrdinalIgnoreCase) select f.FullName;
        }

        /// <summary>
        /// Delete the hostfile profile with the specified <paramref name="profilePath"/>.
        /// </summary>
        /// <param name="profilePath">The file path the host file profile.</param>
        /// <returns>True if the profile with specified <paramref name="profilePath"/> has been deleted from the users hard-disk; otherwise false.</returns>
        public bool DeleteProfile(string profilePath)
        {
            if (this.IsHostFileProfile(profilePath))
            {
                File.Delete(profilePath);
                return true;
            }

            return false;
        }

        /// <summary>Get the full path of the current profile-directory.</summary>
        /// <returns>The full path of the current profile-directory (e.g. "C:\Programs\HostfileManager\"); null if the profile directory was not found.</returns>
        public string GetProfileDirectory()
        {
            DirectoryInfo profileDirectoryInfo = GetDirectoryInfo(this.profileDirectory);
            return this.profileDirectory != null ? profileDirectoryInfo.FullName : null;
        }

        #endregion

        #region IDisposable members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// <param name="disposing">A flag indicating whether this object is currently disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                // Free other state (managed objects).
            }

            // Free your own state (unmanaged objects).
            // Set large fields to null.
            this.hostsFileWatcher.Dispose();
            this.profilesWatcher.Dispose();

            this.disposed = true;
        }

        #endregion

        #region private static functions

        /// <summary>Check whether the current user has read and write access to the file behind the specified <paramref name="filePath"/>.</summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>True if the current user has read and write access to the file behind the specified <paramref name="filePath"/>; otherwise false.</returns>
        private static bool CanReadWrite(string filePath)
        {
            FileInfo file = GetFileInfo(filePath);
            if (file != null)
            {
                return IsFileAccessible(file, FileIOPermissionAccess.AllAccess) && IsFileLocked(file, FileAccess.ReadWrite) == false;
            }

            return false;
        }

        /// <summary>Check whether the specified <paramref name="file"/> is locked or not.</summary>
        /// <param name="file">The <see cref="FileInfo"/> object of the file to check.</param>
        /// <param name="fileAccessType">The file access type (e.g. read, write).</param>
        /// <returns>True if the specified <paramref name="file"/> is locked; otherwise false.</returns>
        private static bool IsFileLocked(FileInfo file, FileAccess fileAccessType)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, fileAccessType, FileShare.None);
            }
            catch (IOException)
            {
                // the file is unavailable because it is:
                // still being written to
                // or being processed by another thread
                // or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            // file is not locked
            return false;
        }

        /// <summary>Check whether the specified <paramref name="file"/> can be acccessed (read/write).</summary>
        /// <param name="file">The <see cref="FileInfo"/> object of the file to check.</param>
        /// <param name="accessType">The access type (Read / Write).</param>
        /// <returns>True if the file behind the specified <paramref name="file"/> is accessible (read / write); otherwise false.</returns>
        private static bool IsFileAccessible(FileSystemInfo file, FileIOPermissionAccess accessType)
        {
            FileIOPermission f = new FileIOPermission(accessType, file.FullName);
            try
            {
                f.Demand();
                return true;
            }
            catch (SecurityException)
            {
            }

            return false;
        }

        /// <summary>
        /// Get all files in the specified directory.
        /// </summary>
        /// <param name="directoryPath">The path to the directory to look for files in.</param>
        /// <returns>An <see cref="IEnumerable{FileInfo}"/> of <see cref="FileInfo"/> objects.</returns>
        private static IEnumerable<FileInfo> GetFilesInDirectory(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                throw new ArgumentNullException("directoryPath");
            }

            var directoryInfo = new DirectoryInfo(directoryPath);
            return directoryInfo.Exists ? directoryInfo.EnumerateFiles() : default(IEnumerable<FileInfo>);
        }

        /// <summary>
        /// Gets the file name of the specified <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The path to a file (e.g. "C:\Windows\System32\drivers\etc\hosts")</param>
        /// <returns>The file name of the specified <paramref name="filePath"/> (e.g. "C:\Windows\System32\drivers\etc\hosts" => "hosts"); null if something went wrong.</returns>
        private static string GetFileName(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            return fi.Name;
        }

        /// <summary>
        /// Gets the parent folder path of the file with the specified <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The path to a file (e.g. "C:\Windows\System32\drivers\etc\hosts").</param>
        /// <returns>The parent folder path of the file with the specified <paramref name="filePath"/> (e.g. "C:\Windows\System32\drivers\etc\hosts" => "C:\Windows\System32\drivers\etc"); null if something went wrong.</returns>
        private static string GetParentFolder(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            return fi.Directory.FullName;
        }

        /// <summary>Return a <see cref="FileInfo"/> object for the file with the specified <paramref name="filePath"/>.</summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The <see cref="FileInfo"/> object for the file with the specified <paramref name="filePath"/> (if the file exists); otherwise false.</returns>
        private static FileInfo GetFileInfo(string filePath)
        {
            if (FileExists(filePath))
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    return fileInfo;
                }
                catch (ArgumentNullException)
                {
                }
                catch (SecurityException)
                {
                }
                catch (ArgumentException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (PathTooLongException)
                {
                }
                catch (NotSupportedException)
                {
                }
            }

            return null;
        }

        /// <summary>Return a <see cref="DirectoryInfo"/> object for the specified <paramref name="directoryPath"/>.</summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <returns>The <see cref="DirectoryInfo"/> object for the specified <paramref name="directoryPath"/> (if the directory exists); otherwise false.</returns>
        private static DirectoryInfo GetDirectoryInfo(string directoryPath)
        {
            if (DirectoryExists(directoryPath))
            {
                try
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                    return directoryInfo;
                }
                catch (ArgumentNullException)
                {
                }
                catch (SecurityException)
                {
                }
                catch (ArgumentException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (PathTooLongException)
                {
                }
                catch (NotSupportedException)
                {
                }
            }

            return null;
        }

        /// <summary>Check whether the specified <paramref name="filePath"/> exist.</summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>True if the specified <paramref name="filePath"/> exist; otherwise false.</returns>
        private static bool FileExists(string filePath)
        {
            return string.IsNullOrEmpty(filePath) == false && File.Exists(filePath);
        }

        /// <summary>Check whether the specified <paramref name="directoryPath"/> exist.</summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <returns>True if the specified <paramref name="directoryPath"/> exist; otherwise false.</returns>
        private static bool DirectoryExists(string directoryPath)
        {
            return string.IsNullOrEmpty(directoryPath) == false && Directory.Exists(directoryPath);
        }

        /// <summary>
        /// Copy the content of the source file to the specified <paramref name="targetPath"/>
        /// </summary>
        /// <param name="sourcePath">The source file path.</param>
        /// <param name="targetPath">The target file path.</param>
        /// <returns>True if the content was successfully copied; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">The specified <paramref name="sourcePath"/> and <paramref name="targetPath"/> cannot be null or empty.</exception>
        /// <exception cref="FileNotFoundException">The specified <paramref name="sourcePath"/> must exist.</exception>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private static bool CopyFile(string sourcePath, string targetPath)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentNullException("sourcePath");
            }

            if (string.IsNullOrEmpty(targetPath))
            {
                throw new ArgumentNullException("targetPath");
            }

            if (FileExists(sourcePath) == false)
            {
                throw new FileNotFoundException("sourcePath");
            }

            FileInfo fileInfoSource = new FileInfo(sourcePath);
            FileInfo fileInfoTarget = new FileInfo(targetPath);

            if (fileInfoSource.FullName.Equals(fileInfoTarget.FullName) == false)
            {
                File.Copy(fileInfoSource.FullName, fileInfoTarget.FullName, true);
                return true;
            }

            return false;
        }

        #endregion

        #region private functions

        /// <summary>Check whether the specified <paramref name="fileNameOrPath"/> is a hostfile profile.</summary>
        /// <param name="fileNameOrPath">The file name or path.</param>
        /// <returns>True if the specified filename ends with the knwon profile extension; otherwise false.</returns>
        private bool IsHostFileProfile(string fileNameOrPath)
        {
            return string.IsNullOrEmpty(fileNameOrPath) == false && fileNameOrPath.EndsWith(this.profileFileExtension, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Check whether the specified <paramref name="filePath"/> is THE hosts file.</summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>True if the specified <paramref name="filePath"/> is THE hosts file; otherwise false.</returns>
        private bool IsOriginalHostsFile(string filePath)
        {
            return string.IsNullOrEmpty(filePath) == false && filePath.Equals(this.hostsFilePath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Verfiy the specified <paramref name="filePath"/> and check whether it is a hosts-file or at least a profile; otherwise throw an exception.</summary>
        /// <param name="filePath">The file path that shall be verified (e.g. "C:\Windows\System32\drivers\etc\hosts").</param>
        /// <exception cref="ArgumentException">The specified <paramref name="filePath"/> must lead to THE hosts file or an host file profile; otherwise this function will throw an <see cref="ArgumentException"/>.</exception>
        /// <exception cref="FileNotFoundException">The specified <paramref name="filePath"/> does not exist.</exception>
        private void VerifyFile(string filePath)
        {
            if (this.IsOriginalHostsFile(filePath) == false && this.IsHostFileProfile(filePath) == false)
            {
                throw new ArgumentException("The specified file type is not a hosts file or a host file profile and can therefore not be loaded.");
            }

            if (FileExists(filePath) == false)
            {
                throw new FileNotFoundException("The file with the specified file path could not be located.", filePath);
            }
        }

        /// <summary>
        /// Attach a <see cref="FileSystemWatcher"/> event to the file computers hosts-file that fires a <see cref="OnHostFileContentChanged"/> event whenever the file has changed.
        /// </summary>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private void InitHostsFileMonitor()
        {
            string folder = GetParentFolder(this.hostsFilePath);
            string filePattern = GetFileName(this.hostsFilePath);

            if (folder == null || filePattern == null)
            {
                return;
            }

            this.hostsFileWatcher = new FileSystemWatcher
            {
                Path = folder,
                Filter = filePattern,
                NotifyFilter = NotifyFilters.LastWrite
            };
            this.hostsFileWatcher.Changed += this.OnHostFileContentChanged;
            this.hostsFileWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Attach a <see cref="FileSystemWatcher"/> to the folder which holds all hostsfile profiles which fires a <see cref="OnProfilesChanged"/> event whenever a profile has been added or removed from the profiles folder.
        /// </summary>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private void InitProfilesMonitor()
        {
            string filePattern = string.Concat("*", this.profileFileExtension);

            this.profilesWatcher = new FileSystemWatcher
            {
                Path = this.profileDirectory,
                Filter = filePattern,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName
            };

            this.profilesWatcher.Created += this.OnProfilesChanged;
            this.profilesWatcher.Deleted += this.OnProfilesChanged;
            this.profilesWatcher.Renamed += this.OnProfilesChanged;

            this.profilesWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Write the specified <paramref name="content"/> to the <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The path to the file that shall be written to (e.g. "C:\Windows\System32\drivers\etc\hosts").</param>
        /// <param name="content">The new text for the specified file.</param>
        /// <returns>True if the <paramref name="content"/> was successfully written to the specified <paramref name="filePath"/>; otherwise false.</returns>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private bool WriteFileContent(string filePath, string content)
        {
            if (string.IsNullOrEmpty(filePath) == false)
            {
                this.hostsFileWatcher.EnableRaisingEvents = false;
                File.WriteAllText(filePath, content);
                this.hostsFileWatcher.EnableRaisingEvents = true;

                return true;
            }

            return false;
        }

        #endregion

        #region events

        /// <summary>
        /// Fire the <see cref="HostFileContentChanged"/> event whenever the contents the computers hosts file have changed.
        /// </summary>
        /// <param name="source">The source <see cref="object"/>.</param>
        /// <param name="e">A <see cref="FileSystemEventArgs"/> that contains the event data.</param>
        private void OnHostFileContentChanged(object source, FileSystemEventArgs e)
        {
            if (this.HostFileContentChanged != null && CanReadWrite(e.FullPath))
            {
                this.HostFileContentChanged(this, e);
            }
        }

        /// <summary>
        /// Fire the <see cref="ProfilesChanged"/> event whenever an profile has been added or removed.
        /// </summary>
        /// <param name="source">The source <see cref="object"/>.</param>
        /// <param name="e">A <see cref="FileSystemEventArgs"/> that contains the event data.</param>
        private void OnProfilesChanged(object source, FileSystemEventArgs e)
        {
            if (this.ProfilesChanged != null)
            {
                this.ProfilesChanged(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
