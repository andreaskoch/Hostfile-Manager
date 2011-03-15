namespace HostfileManager.Model.Base
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// The <see cref="HostfileEntry"/> is the common denominator
    /// for all objects in a hosts file.
    /// </summary>
    public class HostfileEntry : IHostfileEntry, INotifyPropertyChanged
    {
        #region private fields

        /// <summary>A unique identifier for this <see cref="HostfileEntry"/>.</summary>
        private Guid uniqueIdentifier;

        /// <summary>A unique string signature for this <see cref="HostfileEntry"/>.</summary>
        private string signature;

        /// <summary>The childs of this <see cref="HostfileEntry"/>.</summary>
        private HostfileEntryCollection childs;

        /// <summary>The name of this <see cref="HostfileEntry"/>.</summary>
        private string name;

        /// <summary>The description text of this <see cref="HostfileEntry"/>.</summary>
        private string description;

        /// <summary>A flag indicating whether this <see cref="HostfileEntry"/> is currently selected or not (default = false).</summary>
        private bool isSelected;

        /// <summary>A flag indicating whether this <see cref="HostfileEntry"/> is currently expanded or not (default = false).</summary>
        private bool isExpanded;

        #endregion

        #region constructor(s)

        /// <summary>Initializes a new instance of the <see cref="HostfileEntry"/> class. </summary>
        /// <param name="type">The entry type (e.g. <see cref="HostfileEntryType.Host"/>).</param>
        /// <param name="parent">The parent.</param>
        /// <param name="name">The name of this <see cref="HostfileEntry"/> instance.</param>
        /// <param name="propertyChangedCallBack">This event is fired whenever a property of this object changes.</param>
        protected HostfileEntry(HostfileEntryType type, IHostfileEntry parent, string name, EventHandler propertyChangedCallBack)
        {
            if (type.Equals(HostfileEntryType.NotSet) || name == null)
            {
                throw new ArgumentException("name");
            }

            this.EntryType = type;
            this.Name = name;
            this.Parent = parent;

            this.PropertyChangedCallBack = propertyChangedCallBack;
        }

        /// <summary>Initializes a new instance of the <see cref="HostfileEntry"/> class. </summary>
        /// <param name="type">The entry type (e.g. <see cref="HostfileEntryType.Host"/>).</param>
        /// <param name="parent">The parent.</param>
        /// <param name="name">The name of this <see cref="HostfileEntry"/> instance.</param>
        /// <param name="description">A description text.</param>
        /// <param name="childs">The childs.</param>
        /// <param name="propertyChangedCallBack">This event is fired whenever a property of this object changes.</param>
        protected HostfileEntry(HostfileEntryType type, IHostfileEntry parent, string name, string description, HostfileEntryCollection childs, EventHandler propertyChangedCallBack)
        {
            if (type.Equals(HostfileEntryType.NotSet) || string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name");
            }

            this.EntryType = type;
            this.Name = name;
            this.Description = description;
            this.Parent = parent;
            this.Childs = childs;

            this.PropertyChangedCallBack = propertyChangedCallBack;
        }

        #endregion

        #region public events

        /// <summary>This event is fired whenever a property of this object changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Gets or sets the event that is fired whenever a property of this object changes.</summary>
        public EventHandler PropertyChangedCallBack { get; protected set;  }

        #endregion

        #region public IHostfileEntry properties

        /// <summary>Gets or sets the type of this <see cref="HostfileEntry"/>.</summary>
        public HostfileEntryType EntryType { get; set; }

        /// <summary>Gets or sets the name of this <see cref="HostfileEntry"/>.</summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
                this.ThrowOnPropertyChangedEvent("Name");
            }
        }

        /// <summary>Gets or sets the description text of this <see cref="HostfileEntry"/>.</summary>
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.description = value;
                this.ThrowOnPropertyChangedEvent("Description");
            }
        }

        /// <summary>Gets or sets the childs of this <see cref="HostfileEntry"/>.</summary>
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

        /// <summary>Gets a unique identifier for this <see cref="HostfileEntry"/>.</summary>
        public Guid UniqueIdentifier
        {
            get
            {
                if (this.uniqueIdentifier.Equals(Guid.Empty))
                {
                    this.uniqueIdentifier = Guid.NewGuid();
                }

                return this.uniqueIdentifier;                
            }
        }

        /// <summary>Gets a unique string signature for this <see cref="HostfileEntry"/>.</summary>
        public string Signature
        {
            get
            {
                if (string.IsNullOrEmpty(this.signature))
                {
                    this.signature = string.Format("{0}-{1}", this.EntryType, this.GetHashCode());
                }

                return this.signature;
            }
        }

        /// <summary>Gets or sets the parent <see cref="HostfileEntry"/> object.</summary>
        public IHostfileEntry Parent { get; set; }

        /// <summary>Gets a value indicating whether this <see cref="IHostfileEntry"/> has a parent object or not.</summary>
        public bool HasParent
        {
            get
            {
                return this.Parent != null;
            }
        }

        /// <summary>Gets or sets a value indicating whether this <see cref="HostfileEntry"/> is activatable (default = false).</summary>
        public bool IsActivatable { get; protected set; }

        #endregion

        #region public ui properties

        /// <summary>Gets or sets a value indicating whether this <see cref="HostfileEntry"/> is currently selected or not (default = false).</summary>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                this.isSelected = value;
                this.OnPropertyChanged("IsSelected");
            }
        }

        /// <summary>Gets or sets a value indicating whether this <see cref="HostfileEntry"/> is currently expanded or not (default = false).</summary>
        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }

            set
            {
                this.isExpanded = value;
                this.OnPropertyChanged("IsExpanded");
            }
        }

        #endregion

        #region public functions

        /// <summary>Add a child to this <see cref="IHostfileEntry"/>.</summary>
        /// <param name="child">The child entry.</param>
        /// <returns>True if the specified <paramref name="child"/> was added to the list of childs; otherwise false.</returns>
        public bool AddChild(IHostfileEntry child)
        {
            if (this.Childs != null)
            {
                this.Childs.Add(child);
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
                return true;
            }

            return false;
        }

        /// <summary>Delete this <see cref="IHostfileEntry"/>.</summary>
        /// <returns>True if this <see cref="IHostfileEntry"/> was successfully deleted; otherwise false.</returns>
        public bool Delete()
        {
            return this.Parent != null && this.Parent.DeleteChild(this);
        }

        /// <summary>Delete the specified <see cref="IHostfileEntry"/>.</summary>
        /// <param name="hostfileEntry">A <see cref="IHostfileEntry"/>.</param>
        /// <returns>True if the specified <see cref="IHostfileEntry"/> was successfully deleted; otherwise false.</returns>
        public bool DeleteChild(IHostfileEntry hostfileEntry)
        {
            if (this.Childs != null && this.Childs.Count > 0)
            {
                this.Childs.Remove(hostfileEntry.UniqueIdentifier);
            }

            return false;            
        }

        /// <summary>Move the <see cref="IHostfileEntry"/> up.</summary>
        /// <returns>True if this <see cref="IHostfileEntry"/> was moved up successfully; otherwise false.</returns>
        public bool MoveUp()
        {
            return this.Parent != null && this.Parent.MoveChildUp(this.UniqueIdentifier);
        }

        /// <summary>Move the <see cref="IHostfileEntry"/> down.</summary>
        /// <returns>True if this <see cref="IHostfileEntry"/> was moved down successfully; otherwise false.</returns>
        public bool MoveDown()
        {
            return this.Parent != null && this.Parent.MoveChildDown(this.UniqueIdentifier);
        }

        /// <summary>Move the child with the specifid <paramref name="hostfileEntryIdentifier"/> up.</summary>
        /// <param name="hostfileEntryIdentifier">The unique identifier of the <see cref="IHostfileEntry"/> to shall be moved up.</param>
        /// <returns>True if the <see cref="IHostfileEntry"/> with the specified <paramref name="hostfileEntryIdentifier"/> was moved up successfully; otherwise false.</returns>
        public bool MoveChildUp(Guid hostfileEntryIdentifier)
        {
            if (this.Childs != null && this.Childs.Count > 0)
            {
                this.Childs.MoveUp(hostfileEntryIdentifier);
            }

            return false;
        }

        /// <summary>Move the child with the specifid <paramref name="hostfileEntryIdentifier"/> down.</summary>
        /// <param name="hostfileEntryIdentifier">The unique identifier of the <see cref="IHostfileEntry"/> to shall be moved down.</param>
        /// <returns>True if the <see cref="IHostfileEntry"/> with the specified <paramref name="hostfileEntryIdentifier"/> was moved down successfully; otherwise false.</returns>
        public bool MoveChildDown(Guid hostfileEntryIdentifier)
        {
            if (this.Childs != null && this.Childs.Count > 0)
            {
                this.Childs.MoveDown(hostfileEntryIdentifier);
            }

            return false;            
        }

        /// <summary>
        /// Get the text-representation of this object instance.
        /// </summary>
        /// <returns>The text-representation of the current object instance.</returns>
        public virtual string GetText()
        {
            return string.Concat(Constants.HostFileSyntaxCommentCharacter, " ", this.EntryType, ": ", this.Name);
        }

        /// <summary>Throw the OnPropertyChanged event.</summary>
        /// <param name="propertyName">The property name.</param>
        public void ThrowOnPropertyChangedEvent(string propertyName)
        {
            this.OnPropertyChanged(propertyName);
        }

        #endregion

        #region object overrides

        /// <summary>Returns a string that represents the current <see cref="object"/>.</summary>
        /// <returns>A string that represents the current <see cref="object"/>.</returns>
        public override string ToString()
        {
            return string.Format("{0} (Name: {1}, UniqueIdentifier: {2})", this.EntryType, this.Name, this.UniqueIdentifier);
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
                var otherObj = obj as IHostfileEntry;
                if (otherObj != null)
                {
                    return this.UniqueIdentifier.Equals(otherObj.UniqueIdentifier);
                }
            }

            return false;
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

            if (this.PropertyChangedCallBack != null)
            {
                this.PropertyChangedCallBack(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
