namespace HostfileManager.ViewModel
{
    using System;
    using System.IO;

    using HostfileManager.Logic;
    using HostfileManager.Model;
    using HostfileManager.Model.Base;
    using HostfileManager.View;

    /// <summary>
    /// The ViewModel class for the <see cref="EditorView"/> user-interface.
    /// </summary>
    public class HostsFileViewModel : ViewModelBase
    {
        #region private members

        /// <summary>The <see cref="HostFile"/> object for this view.</summary>
        private HostFile hostFileInstance;

        /// <summary>
        /// A flag indicating wehther this object has been disposed.
        /// </summary>
        private bool disposed;

        #endregion

        #region constructor(s)

        /// <summary>Initializes a new instance of the <see cref="HostsFileViewModel"/> class.</summary>
        public HostsFileViewModel()
        {
            /* attach content-changed event */
            HostfileManager.Current.HostFileContentChanged += this.HostsFileContentChanged;
        }

        #endregion

        #region public events

        /// <summary>Event that is raised when the <see cref="HostFileInstance"/> property changed.</summary>
        public event EventHandler<EventArgs> HostsFileObjectChanged;

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the <see cref="HostFile"/> for this ViewModel.
        /// </summary>
        public HostFile HostFileInstance
        {
            get
            {
                return this.hostFileInstance ?? (this.hostFileInstance = HostfileManager.Current.GetHostsFile());
            }

            set
            {
                this.hostFileInstance = value;

                OnPropertyChanged(null);
                this.InvokeHostsFileObjectChanged();
            }
        }

        #endregion

        #region public functions and methods

        /// <summary>Restores the default hosts-file content.</summary>
        /// <returns>True if the default hosts-file could be restored; otherwise false.</returns>
        public bool RestoreDefaultHostsFile()
        {
            bool result = HostfileManager.Current.RestoreDefaultHostsFile();
            if (result)
            {
                this.ReloadFromDisk();
            }

            return result;
        }

        /// <summary>
        /// Loads the specified <see cref="HostsFileProfile"/>.
        /// </summary>
        /// <param name="sourceProfile">A <see cref="HostsFileProfile"/> object.</param>
        /// <returns>True if the specified <paramref name="sourceProfile"/> was loaded successfully; otherwise false.</returns>
        public bool LoadProfile(HostsFileProfile sourceProfile)
        {
            if (this.HostFileInstance != null)
            {
                HostFile hf = HostfileManager.Current.GetProfile(sourceProfile.ProfilePath);
                if (hf != null)
                {
                    this.HostFileInstance = hf;
                    return true;
                }
            }

            return false;
        }

        /// <summary>Loads the hosts file with specified <paramref name="filePath"/>.</summary>
        /// <param name="filePath">The file path to a hosts file or a hosts file profile.</param>
        /// <returns>True if the specified <paramref name="filePath"/> was loaded successfully; otherwise false.</returns>
        public bool LoadFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath");
            }

            if (this.HostFileInstance != null)
            {
                HostFile hf = HostfileManager.Current.GetHostsFile(filePath);
                if (hf != null)
                {
                    this.HostFileInstance = hf;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Save the current <see cref="HostFileInstance"/> back to the current user's hosts file.
        /// </summary>
        /// <returns>True if the data was successfully exported and saved; otherwise false.</returns>
        public bool SaveToDisk()
        {
            return this.SaveToDisk(this.HostFileInstance.FilePath);
        }

        /// <summary>Save the current <see cref="HostFileInstance"/> back to the current user's hosts file.</summary>
        /// <param name="filepath">The filepath.</param>
        /// <returns>True if the data was successfully exported and saved; otherwise false.</returns>
        public bool SaveToDisk(string filepath)
        {
            this.HostFileInstance.FilePath = filepath;
            bool result = HostfileManager.Current.SaveHostsFileContent(this.HostFileInstance, filepath);

            if (result)
            {
                this.InvokeHostsFileObjectChanged();
            }

            return result;
        }

        /// <summary>Save the specified <paramref name="text"/> back to the current user's hosts file.</summary>
        /// <param name="filepath">The filepath.</param>
        /// <param name="text">The new hosts file content.</param>
        /// <returns>True if the specified <paramref name="text"/> was successfully saved; otherwise false.</returns>
        public bool SaveToDisk(string filepath, string text)
        {
            if (text != null)
            {
                this.HostFileInstance.FilePath = filepath;
                if (HostfileManager.Current.SaveHostsFileContent(text, filepath))
                {
                    this.ReloadFromDisk(filepath);
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>Reloads the content of the hosts file from disk and updates the user-interface.</summary>
        public void ReloadFromDisk()
        {
            this.ReloadFromDisk(this.HostFileInstance.FilePath);
        }

        /// <summary>Reloads the content of the supplied hosts file from disk and updates the user-interface.</summary>
        /// <param name="filepath">The full path to a hosts file or a hosts file profile.</param>
        public void ReloadFromDisk(string filepath)
        {
            FileInfo fileInfo = new FileInfo(filepath);
            this.HostsFileContentChanged(this, new FileSystemEventArgs(WatcherChangeTypes.Changed, fileInfo.Directory.FullName, fileInfo.Name));
        }

        /// <summary>Gets the text-representation of the current <see cref="HostFileInstance"/>.</summary>
        /// <returns>The text-representation of the current <see cref="HostFileInstance"/>.</returns>
        public string GetText()
        {
            return HostfileManager.Current.ConvertHostfileToText(this.HostFileInstance);
        }

        /// <summary>Raises the <see cref="HostsFileObjectChanged"/> event.</summary>
        public void InvokeHostsFileObjectChanged()
        {
            this.InvokeHostsFileObjectChanged(EventArgs.Empty);
        }

        /// <summary>Raises the <see cref="HostsFileObjectChanged"/> event.</summary>
        /// <param name="e">The <see cref="EventArgs"/> event parameter object.</param>
        public void InvokeHostsFileObjectChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = this.HostsFileObjectChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>Toggle the active state of the specified <paramref name="activatableHostfileEntry"/>.</summary>
        /// <param name="activatableHostfileEntry">A <see cref="IHostfileEntry"/> object.</param>
        /// <returns>True if the active state of the specified <see cref="IHostfileEntry"/> was successfully toggled; otherwise false.</returns>
        public bool ToggleActiveState(ActivatableHostfileEntry activatableHostfileEntry)
        {
            /* abort if parameter is null */
            if (activatableHostfileEntry == null)
            {
                return false;
            }

            /* toggle host status */
            if (activatableHostfileEntry.HasParent)
            {
                activatableHostfileEntry.ToggleActiveState();
                return true;
            }

            /* toggle group status */
            return this.HostFileInstance.ToggleActiveState(activatableHostfileEntry.UniqueIdentifier);
        }

        /// <summary>Remove all <see cref="IHostfileEntry"/> objects from the current <see cref="HostFileInstance"/>.</summary>
        /// <returns>True if the all <see cref="IHostfileEntry"/> objects have been removed from the current <see cref="HostFileInstance"/>; otherwise false.</returns>
        public bool Clear()
        {
            return this.HostFileInstance != null && this.HostFileInstance.Clear();
        }

        /// <summary>Remove the specified <paramref name="hostfileEntry"/>.</summary>
        /// <param name="hostfileEntry">A <see cref="IHostfileEntry"/> object.</param>
        /// <returns>True if the specified <paramref name="hostfileEntry"/> was successfully removed; otherwise false.</returns>
        public bool Remove(IHostfileEntry hostfileEntry)
        {
            if (hostfileEntry != null)
            {
                if (hostfileEntry.HasParent)
                {
                    hostfileEntry.Delete();
                }
                else
                {
                    this.HostFileInstance.Remove(hostfileEntry.UniqueIdentifier);
                }
            }

            return false;
        }

        /// <summary>
        /// Move the specified <paramref name="hostfileEntry"/> up.
        /// </summary>
        /// <param name="hostfileEntry">A <see cref="IHostfileEntry"/> object.</param>
        /// <returns>True if the specified <paramref name="hostfileEntry"/> was successfully moved up; otherwise false.</returns>
        public bool MoveUp(IHostfileEntry hostfileEntry)
        {
            if (hostfileEntry != null)
            {
                if (hostfileEntry.HasParent)
                {
                    hostfileEntry.MoveUp();
                }
                else
                {
                    this.HostFileInstance.MoveUp(hostfileEntry.UniqueIdentifier);
                }
            }

            return false;
        }

        /// <summary>
        /// Move the specified <paramref name="hostfileEntry"/> down.
        /// </summary>
        /// <param name="hostfileEntry">A <see cref="IHostfileEntry"/> object.</param>
        /// <returns>True if the specified <paramref name="hostfileEntry"/> was successfully moved down; otherwise false.</returns>
        public bool MoveDown(IHostfileEntry hostfileEntry)
        {
            if (hostfileEntry != null)
            {
                if (hostfileEntry.HasParent)
                {
                    hostfileEntry.MoveDown();
                }
                else
                {
                    this.HostFileInstance.MoveDown(hostfileEntry.UniqueIdentifier);
                }
            }

            return false;
        }

        /// <summary>Add an empty <see cref="Comment"/> object instance to the current <see cref="HostFileInstance"/>.</summary>
        /// <returns>True if a new <see cref="Comment"/> object instance has been successfully added to the current <see cref="HostFileInstance"/>; otherwise false.</returns>
        public bool AddComment()
        {
            return this.HostFileInstance != null && this.HostFileInstance.AddChildToTop(Comment.CreateEmpty(this.HostFileInstance.PropertyChangedCallBack));
        }

        /// <summary>Add an empty <see cref="HostGroup"/> object instance to the current <see cref="HostFileInstance"/>.</summary>
        /// <returns>True if a new <see cref="HostGroup"/> object instance has been successfully added to the current <see cref="HostFileInstance"/>; otherwise false.</returns>
        public bool AddGroup()
        {
            return this.HostFileInstance != null && this.HostFileInstance.AddChildToTop(HostGroup.CreateEmpty(this.HostFileInstance.PropertyChangedCallBack));
        }

        /// <summary>Add an empty <see cref="Host"/> object instance to the current <see cref="HostFileInstance"/>.</summary>
        /// <param name="parent">The parent of the to be created <see cref="Host"/> object (optional, default = null).</param>
        /// <returns>True if a new <see cref="Host"/> object instance has been successfully added to the current <see cref="HostFileInstance"/>; otherwise false.</returns>
        public bool AddHost(HostGroup parent = null)
        {
            if (this.HostFileInstance != null)
            {
                if (parent != null)
                {
                    return parent.AddChildToTop(Host.CreateEmpty(parent, this.HostFileInstance.PropertyChangedCallBack));
                }

                return this.HostFileInstance.AddChildToTop(Host.CreateEmpty(parent, this.HostFileInstance.PropertyChangedCallBack));
            }

            return false;
        }

        /// <summary>Add an empty <see cref="Domain"/> object instance to the current <see cref="HostFileInstance"/>.</summary>
        /// <param name="parent">The parent of the to be created <see cref="Domain"/> object (optional, default = null).</param>
        /// <returns>True if a new <see cref="Domain"/> object instance has been successfully added to the current <see cref="HostFileInstance"/>; otherwise false.</returns>
        public bool AddDomain(Host parent)
        {
            if (parent != null && this.HostFileInstance != null)
            {
                return parent.AddChildToTop(Domain.CreateEmpty(parent, this.HostFileInstance.PropertyChangedCallBack));
            }

            return false;
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
                    this.hostFileInstance = null;
                    HostfileManager.Current.Dispose();
                }

                // Free your own state (unmanaged objects).
                // Set large fields to null.
                this.disposed = true;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region private function, methods and events

        /// <summary>
        /// Updates the user-interface with the values from the new hosts-file content.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="FileSystemEventArgs"/> that contains the event data.</param>    
        private void HostsFileContentChanged(object sender, FileSystemEventArgs e)
        {
            if (string.IsNullOrEmpty(e.FullPath) || File.Exists(e.FullPath) == false)
            {
                return;
            }

            HostFile newHostsFile = HostfileManager.Current.GetHostsFile(e.FullPath);
            if (newHostsFile != null)
            {
                /* attach new data-model */
                this.HostFileInstance = newHostsFile;
            }
        }

        #endregion
    }
}
