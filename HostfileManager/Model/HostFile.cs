namespace HostfileManager.Model
{
    using System;
    using System.ComponentModel;
    using System.Text;

    using HostfileManager.Model.Base;

    /// <summary>
    /// The <see cref="HostFile"/> class implements the <see cref="INotifyPropertyChanged"/>
    /// interface and is the representation of a static hosts file and
    /// holds the references to all <see cref="IHostfileEntry"/> objects.
    /// </summary>
    public class HostFile : INotifyPropertyChanged
    {
        #region private fields

        /// <summary>A flag indicating whether the <see cref="Text"/> property of this <see cref="HostFile"/> instance has changed (default = true).</summary>
        private bool textPropertyHasChanged = true;

        /// <summary>The text of this <see cref="HostFile"/> instance.</summary>
        private string text;

        /// <summary>The <see cref="HostfileEntryCollection"/> of this <see cref="HostFile"/> instance.</summary>
        private HostfileEntryCollection childs;

        /// <summary>A flag indicating whether the group toggle-mode is enabled or not (default = false).</summary>
        private bool exclusiveGroupToggleModeIsEnabled;

        /// <summary>A flag indicating whether the host toggle-mode is enabled or not (default = false)</summary>
        private bool exclusiveHostToggleModeIsEnabled;

        #endregion

        #region constructor(s)

        /// <summary>Prevents a default instance of the <see cref="HostFile"/> class from being created.</summary>
        private HostFile()
        {
        }

        #endregion

        #region public events

        /// <summary>This event is fired whenever a property of this object changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region properties

        /// <summary>Gets or sets the text of this <see cref="HostFile"/> instance.</summary>
        public string Text
        {
            get
            {
                if (this.textPropertyHasChanged || this.text == null)
                {
                    this.text = this.GetText();
                    this.textPropertyHasChanged = false;
                }

                return this.text;
            }

            set
            {
                this.text = value;
            }
        }

        /// <summary>Gets or sets the file path of this <see cref="HostFile"/> instance.</summary>
        public string FilePath { get; set; }

        /// <summary>Gets or sets the <see cref="HostfileEntryCollection"/> of this <see cref="HostFile"/> instance.</summary>
        public HostfileEntryCollection Childs
        {
            get
            {
                return this.childs ?? (this.childs = new HostfileEntryCollection(this.PropertyChangedCallBack));
            }

            set
            {
                this.childs = value;
            }
        }

        #endregion

        #region option properties

        /// <summary>Gets or sets a value indicating whether the group toggle-mode is enabled or not (default = false).</summary>
        public bool ExclusiveGroupToggleModeIsEnabled
        {
            get
            {
                return this.exclusiveGroupToggleModeIsEnabled;
            }

            set
            {
                this.exclusiveGroupToggleModeIsEnabled = value;
                this.OnPropertyChanged("ExclusiveGroupToggleModeIsEnabled");
                this.UpdateTextProperty();
            }
        }

        /// <summary>Gets or sets a value indicating whether the host toggle-mode is enabled or not (default = false).</summary>
        public bool ExclusiveHostToggleModeIsEnabled
        {
            get
            {
                return this.exclusiveHostToggleModeIsEnabled;
            }

            set
            {
                this.exclusiveHostToggleModeIsEnabled = value;
                this.OnPropertyChanged("ExclusiveHostToggleModeIsEnabled");
                this.UpdateTextProperty();
            }
        }

        #endregion

        #region factory methods

        /// <summary>Create a new <see cref="HostFile"/> object.</summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>An initialized <see cref="HostFile"/> object instance.</returns>
        public static HostFile CreateHostFile(string filePath)
        {
            return new HostFile { FilePath = filePath };
        }

        /// <summary>Create a new <see cref="HostFile"/> object.</summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="entries">The entries.</param>
        /// <returns>An initialized <see cref="HostFile"/> object instance.</returns>
        public static HostFile CreateHostFile(string filePath, HostfileEntryCollection entries)
        {
            return new HostFile { FilePath = filePath, Childs = entries };
        }

        #endregion

        #region object overrides

        /// <summary>Returns a string that represents the current <see cref="object"/>.</summary>
        /// <returns>A string that represents the current <see cref="object"/>.</returns>
        public override string ToString()
        {
            return string.Format("HostFile (FilePath: {0})", this.FilePath);
        }

        /// <summary>Serves as a hash function for a particular type.</summary>
        /// <returns>A hash code for the current <see cref="object"/>.</returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>Determines whether the specified Object is equal to the current <see cref="object"/>.</summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="object"/>.</param>
        /// <returns>true if the specified Object is equal to the current Object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                var otherObj = obj as HostFile;
                if (otherObj != null)
                {
                    return this.GetText() == otherObj.GetText();
                }
            }

            return false;
        }

        #endregion

        #region public callback methods

        /// <summary>The callback method for property-changed events.</summary>
        /// <param name="sender">The sender <see cref="object"/>.</param>
        /// <param name="args">The <see cref="EventArgs"/> event parameters.</param>
        public void PropertyChangedCallBack(object sender, EventArgs args)
        {
            this.UpdateTextProperty();
        }

        #endregion

        #region public functions

        /// <summary>
        /// Get the text-representation of this object instance.
        /// </summary>
        /// <returns>The text-representation of the current object instance.</returns>
        public string GetText()
        {
            StringBuilder hostfileText = new StringBuilder();

            /* global options */
            if (this.ExclusiveGroupToggleModeIsEnabled || this.ExclusiveHostToggleModeIsEnabled)
            {
                if (this.ExclusiveGroupToggleModeIsEnabled)
                {
                    hostfileText.AppendLine(string.Concat(Constants.HostFileSyntaxCommentCharacter, " ", Constants.HostFileSyntaxControlElementGroupToggleMode));
                }

                if (this.ExclusiveHostToggleModeIsEnabled)
                {
                    hostfileText.AppendLine(string.Concat(Constants.HostFileSyntaxCommentCharacter, " ", Constants.HostFileSyntaxControlElementHostToggleMode));
                }

                hostfileText.AppendLine();
            }

            /* hostfile entries */
            if (this.Childs != null)
            {
                foreach (HostfileEntry item in this.Childs)
                {
                    hostfileText.AppendLine(item.GetText());
                }                
            }

            return hostfileText.ToString();
        }

        /// <summary>Toggle the active state of the <see cref="IHostfileEntry"/> with the specified <paramref name="uniqueIdentifier"/>.</summary>
        /// <param name="uniqueIdentifier">The unique identifier of an <see cref="IHostfileEntry"/> object.</param>
        /// <returns>True if the active state of the <see cref="IHostfileEntry"/> with the specified <paramref name="uniqueIdentifier"/> was successfully toggled; otherwise false.</returns>
        public bool ToggleActiveState(Guid uniqueIdentifier)
        {
            /* abort if there are no childs in this host file */
            if (this.Childs == null || this.Childs.Count == 0)
            {
                return false;
            }

            /* get the host file entry with the specified unique identifier */
            IHostfileEntry hostFileEntry = this.Childs.GetElementBy(uniqueIdentifier);
            if (hostFileEntry == null || hostFileEntry.IsActivatable == false)
            {
                /* abort if the host file entry was not found or is not activatable */
                return false;
            }

            /* cast the current host file entry to an activatable hostfile entry */
            ActivatableHostfileEntry activatableHostfileEntry = hostFileEntry as ActivatableHostfileEntry;
            if (activatableHostfileEntry == null)
            {
                /* abort if the host file entry is not an ActivatableHostfileEntry */
                return false;
            }

            /* variables */
            bool isGroup = activatableHostfileEntry.EntryType.Equals(HostfileEntryType.HostGroup);
            bool isHost = activatableHostfileEntry.EntryType.Equals(HostfileEntryType.Host);

            /* new states */
            bool previousState = activatableHostfileEntry.IsActive;
            bool newState = !previousState;

            /* no exclusive toggle mode: groups */
            if (isGroup && this.ExclusiveGroupToggleModeIsEnabled == false)
            {
                activatableHostfileEntry.IsActive = newState;
                return true;
            }

            /* no exclusive toggle mode: hosts */
            if (isHost && this.ExclusiveHostToggleModeIsEnabled == false)
            {
                activatableHostfileEntry.IsActive = newState;
                return true;
            }

            /* just deactivate the current entry */
            if (newState == false && this.ExclusiveGroupToggleModeIsEnabled == false && this.ExclusiveHostToggleModeIsEnabled == false)
            {
                activatableHostfileEntry.IsActive = false;
                return true;
            }

            /* deactivate the current entry and all other groups */
            if (newState == false && isGroup && this.ExclusiveGroupToggleModeIsEnabled)
            {
                foreach (HostGroup entry in this.Childs.Groups)
                {
                    entry.IsActive = false;
                }

                return true;
            }

            /* deactivate the current entry and all other hosts */
            if (newState == false && isHost && this.ExclusiveHostToggleModeIsEnabled)
            {
                foreach (Host entry in this.Childs.Hosts)
                {
                    entry.IsActive = false;
                }

                return true;
            }
            
            /* toggle states: groups only */
            if (isGroup && this.ExclusiveGroupToggleModeIsEnabled)
            {
                foreach (HostGroup entry in this.Childs.Groups)
                {
                    entry.IsActive = entry.UniqueIdentifier.Equals(uniqueIdentifier) ? true : false;
                }
            }

            /* toggle states: hosts only */
            if (isHost && this.ExclusiveHostToggleModeIsEnabled)
            {
                foreach (Host entry in this.Childs.Hosts)
                {
                    entry.IsActive = entry.UniqueIdentifier.Equals(uniqueIdentifier) ? true : false;
                }
            }

            return true;
        }

        /// <summary>Remove all <see cref="IHostfileEntry"/> objects.</summary>
        /// <returns>True if the all <see cref="IHostfileEntry"/> objects have been removed; otherwise false.</returns>
        public bool Clear()
        {
            if (this.Childs != null && this.Childs.Count > 0)
            {
                this.Childs.Clear();

                this.OnPropertyChanged("Childs");
                return true;
            }

            return false;
        }

        /// <summary>Add a child to the top of this <see cref="HostFile"/> instance.</summary>
        /// <param name="child">The child entry.</param>
        /// <returns>True if the specified <paramref name="child"/> was added to the list of childs; otherwise false.</returns>
        public bool AddChildToTop(IHostfileEntry child)
        {
            if (this.Childs != null)
            {
                this.Childs.AddChildToTop(child);

                this.OnPropertyChanged("Childs");
                return true;
            }

            return false;
        }

        /// <summary>Add a child to this <see cref="HostFile"/> instance.</summary>
        /// <param name="child">The child entry.</param>
        /// <returns>True if the specified <paramref name="child"/> was added to the list of childs; otherwise false.</returns>
        public bool AddChild(IHostfileEntry child)
        {
            if (this.Childs != null)
            {
                this.Childs.Add(child);

                this.OnPropertyChanged("Childs");
                return true;
            }

            return false;
        }

        /// <summary>Remove the <see cref="IHostfileEntry"/> with the specified <paramref name="uniqueIdentifier"/>.</summary>
        /// <param name="uniqueIdentifier">The unique identifier of an <see cref="IHostfileEntry"/> object.</param>
        /// <returns>True if the <see cref="IHostfileEntry"/> with the specified <paramref name="uniqueIdentifier"/> was successfully removed; otherwise false.</returns>
        public bool Remove(Guid uniqueIdentifier)
        {
            if (this.Childs != null && this.Childs.Count > 0)
            {
                bool result = this.Childs.Remove(uniqueIdentifier);

                this.OnPropertyChanged("Childs");
                return result;
            }

            return false;
        }

        /// <summary>
        /// Move the <see cref="IHostfileEntry"/> with the specified <paramref name="uniqueIdentifier"/> up.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier of an <see cref="IHostfileEntry"/> object.</param>
        /// <returns>The new index of the <see cref="IHostfileEntry"/> object with the specified <paramref name="uniqueIdentifier"/>; -1 if the specified <paramref name="uniqueIdentifier"/> was not found.</returns>
        public int MoveUp(Guid uniqueIdentifier)
        {
            if (this.Childs != null && this.Childs.Count > 0)
            {
                int result = this.Childs.MoveUp(uniqueIdentifier);

                this.OnPropertyChanged("Childs");
                return result;
            }

            return -1;
        }

        /// <summary>
        /// Move the <see cref="IHostfileEntry"/> with the specified <paramref name="uniqueIdentifier"/> down.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier of an <see cref="IHostfileEntry"/> object.</param>
        /// <returns>The new index of the <see cref="IHostfileEntry"/> object with the specified <paramref name="uniqueIdentifier"/>; -1 if the specified <paramref name="uniqueIdentifier"/> was not found.</returns>
        public int MoveDown(Guid uniqueIdentifier)
        {
            if (this.Childs != null && this.Childs.Count > 0)
            {
                int result = this.Childs.MoveDown(uniqueIdentifier);

                this.OnPropertyChanged("Childs");
                return result;
            }

            return -1;
        }

        #endregion

        #region INotifyPropertyChanged members

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this FrameworkElement has been updated.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion        

        #region private methods

        /// <summary>Mark the <see cref="Text"/> property for an update.</summary>
        private void UpdateTextProperty()
        {
            this.textPropertyHasChanged = true;
            this.OnPropertyChanged("Text");
        }

        #endregion
    }
}
