namespace HostfileManager.Model.Base
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// The <see cref="HostfileEntryCollection"/> is a
    /// <see cref="ObservableCollection{IHostfileEntry}"/> of <see cref="IHostfileEntry"/> object.
    /// </summary>
    public sealed class HostfileEntryCollection : ObservableCollection<IHostfileEntry>
    {
        #region private fields

        /// <summary>The callback event handler that is fired when a property of this collection changed.</summary>
        private readonly EventHandler propertyChangedCallBack;

        #endregion

        #region constructor(s)

        /// <summary>Initializes a new instance of the <see cref="HostfileEntryCollection"/> class.</summary>
        /// <param name="propertyChangedCallBack">This event is fired whenever a property of this object changes.</param>
        public HostfileEntryCollection(EventHandler propertyChangedCallBack)
        {
            this.propertyChangedCallBack = propertyChangedCallBack;
            this.PropertyChanged += this.OnPropertyChanged;
        }

        #endregion

        #region custom enumerators

        /// <summary>Gets all <see cref="IHostfileEntry"/> objects in this collection which implement the <see cref="IActivatableHostfileEntry"/> interface.</summary>
        public IEnumerable<IHostfileEntry> Activatable
        {
            get
            {
                return this.Where(child => child.IsActivatable);
            }
        }

        /// <summary>Gets all <see cref="HostGroup"/> objects in this collection which implement the <see cref="IActivatableHostfileEntry"/> interface and are of type <see cref="HostfileEntryType.HostGroup"/>.</summary>
        public IEnumerable<HostGroup> Groups
        {
            get
            {
                return this.OfType<HostGroup>().Where(child => child.IsActivatable && child.EntryType.Equals(HostfileEntryType.HostGroup));
            }
        }

        /// <summary>Gets all <see cref="Host"/> objects in this collection which implement the <see cref="IActivatableHostfileEntry"/> interface and are of type <see cref="HostfileEntryType.Host"/>.</summary>
        public IEnumerable<Host> Hosts
        {
            get
            {
                return this.OfType<Host>().Where(child => child.IsActivatable && child.EntryType.Equals(HostfileEntryType.Host));
            }
        }

        #endregion

        #region indexer

        /// <summary>Gets the <see cref="IHostfileEntry"/> which matches the specified <paramref name="uniqueIdentifier"/>.</summary>
        /// <param name="uniqueIdentifier">The unique identifier of the requested <see cref="IHostfileEntry"/>.</param>
        public IHostfileEntry this[Guid uniqueIdentifier]
        {
            get { return this.GetElementBy(uniqueIdentifier); }
        }

        /// <summary>Gets the <see cref="IHostfileEntry"/> which matches the specified <paramref name="signature"/>.</summary>
        /// <param name="signature">The signature of the requested <see cref="IHostfileEntry"/>.</param>
        public IHostfileEntry this[string signature]
        {
            get { return this.GetElementBy(signature); }
        }

        #endregion

        #region public functions

        /// <summary>Returns the zero-based index of the first occurrence of <see cref="IHostfileEntry"/> with the specified <paramref name=" uniqueIdentifier"/>.</summary>
        /// <param name="uniqueIdentifier">The unique identifier of a <see cref="IHostfileEntry"/>.</param>
        /// <returns>The zero-based index of the first occurrence of <see cref="IHostfileEntry"/> with the specified <paramref name=" uniqueIdentifier"/>; -1 if the specified <paramref name="uniqueIdentifier"/> was not found.</returns>
        public int IndexOf(Guid uniqueIdentifier)
        {
            for (var i = 0; i < this.Count; i++)
            {
                if (this[i].UniqueIdentifier.Equals(uniqueIdentifier))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>Returns the <see cref="IHostfileEntry"/> which matches the specified <paramref name="entryType"/> and <paramref name="name"/>.</summary>
        /// <param name="entryType">The entry type of the <see cref="IHostfileEntry"/>.</param>
        /// <param name="name">The name of the <see cref="IHostfileEntry"/>.</param>
        /// <returns>The <see cref="IHostfileEntry"/> which matches the specified <paramref name="entryType"/> and <paramref name="name"/>; null if the <see cref="IHostfileEntry"/> was not found.</returns>
        public IHostfileEntry GetElementBy(HostfileEntryType entryType, string name)
        {
            return this.FirstOrDefault(item => item.EntryType.Equals(entryType) && item.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>Returns the <see cref="IHostfileEntry"/> which matches the specified <paramref name="uniqueIdentifier"/>.</summary>
        /// <param name="uniqueIdentifier">The unique identifier of the <see cref="IHostfileEntry"/>.</param>
        /// <returns>The <see cref="IHostfileEntry"/> which matches the specified <paramref name="uniqueIdentifier"/>; null if the <see cref="IHostfileEntry"/> was not found.</returns>
        public IHostfileEntry GetElementBy(Guid uniqueIdentifier)
        {
            return this.FirstOrDefault(item => item.UniqueIdentifier.Equals(uniqueIdentifier));
        }

        /// <summary>Returns the <see cref="IHostfileEntry"/> which matches the specified <paramref name="signature"/>.</summary>
        /// <param name="signature">The signature of the <see cref="IHostfileEntry"/>.</param>
        /// <returns>The <see cref="IHostfileEntry"/> which matches the specified <paramref name="signature"/>; null if the <see cref="IHostfileEntry"/> was not found.</returns>
        public IHostfileEntry GetElementBy(string signature)
        {
            return this.FirstOrDefault(item => item.Signature.Equals(signature));
        }

        #endregion

        #region public functions

        /// <summary>Add a child to the top of this <see cref="HostFile"/> instance.</summary>
        /// <param name="child">The child entry.</param>
        /// <returns>True if the specified <paramref name="child"/> was added to the list of childs; otherwise false.</returns>
        public bool AddChildToTop(IHostfileEntry child)
        {
            if (child != null)
            {
                this.Insert(0, child);
                return true;   
            }

            return false;
        }

        /// <summary>Remove the <see cref="IHostfileEntry"/> with the specified <paramref name="uniqueIdentifier"/>.</summary>
        /// <param name="uniqueIdentifier">The unique identifier of an <see cref="IHostfileEntry"/> object.</param>
        /// <returns>True if the <see cref="IHostfileEntry"/> with the specified <paramref name="uniqueIdentifier"/> was successfully removed; otherwise false.</returns>
        public bool Remove(Guid uniqueIdentifier)
        {
            int itemIndex = this.IndexOf(uniqueIdentifier);
            if (itemIndex != -1)
            {
                this.RemoveItem(itemIndex);
            }

            return false;
        }

        /// <summary>Toggle the active state of the <see cref="IHostfileEntry"/> with the specified <paramref name="uniqueIdentifier"/>.</summary>
        /// <param name="uniqueIdentifier">The unique identifier of an <see cref="IHostfileEntry"/> object.</param>
        /// <returns>True if the active state of the <see cref="IHostfileEntry"/> with the specified <paramref name="uniqueIdentifier"/> was successfully toggled; otherwise false.</returns>
        public bool ToggleActiveStateObsolete(Guid uniqueIdentifier)
        {
            IHostfileEntry item = this.GetElementBy(uniqueIdentifier);
            if (item != null && item.IsActivatable)
            {
                ActivatableHostfileEntry activatableItem = item as ActivatableHostfileEntry;
                if (activatableItem != null)
                {
                    bool currentState = activatableItem.IsActive;
                    activatableItem.IsActive = !currentState;
                    bool newState = activatableItem.IsActive;

                    return currentState != newState;
                }
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
            int size = this.Count;
            int currentIndex = this.IndexOf(uniqueIdentifier);
            if (currentIndex != -1 && size > 0)
            {
                int nextIndex = GetNextRingPosition(currentIndex, size, -1);
                this.Move(currentIndex, nextIndex);
                return nextIndex;
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
            int size = this.Count;
            int currentIndex = this.IndexOf(uniqueIdentifier);
            if (currentIndex != -1 && size > 0)
            {
                int nextIndex = GetNextRingPosition(currentIndex, size, 1);
                this.Move(currentIndex, nextIndex);
                return nextIndex;
            }

            return -1;
        }

        #endregion

        #region helper functions

        /// <summary>
        /// Calculate the next position in the ring with the given dimensions.
        /// </summary>
        /// <param name="currentIndex">The current position.</param>
        /// <param name="size">The ring size.</param>
        /// <param name="step">The positive or negative step value.</param>
        /// <returns>The next position in the ring.</returns>
        private static int GetNextRingPosition(int currentIndex, int size, int step)
        {
            int nextIndex = currentIndex + step;
            if (nextIndex >= size)
            {
                nextIndex = nextIndex - size;
            }
            else if (nextIndex < 0)
            {
                nextIndex = nextIndex + size;
            }

            return nextIndex;
        }

        #endregion

        #region private event handlers

        /// <summary>This event is executed whever the property-changed event of this <see cref="HostfileEntryCollection"/> instance is called.</summary>
        /// <param name="sender">The sender <see cref="object"/>.</param>
        /// <param name="args">The <see cref="PropertyChangedEventArgs"/> event arguments.</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (this.propertyChangedCallBack != null)
            {
                this.propertyChangedCallBack(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
