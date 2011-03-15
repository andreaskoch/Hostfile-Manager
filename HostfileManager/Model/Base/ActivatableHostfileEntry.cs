namespace HostfileManager.Model.Base
{
    using System;
    using System.Linq;

    /// <summary>
    /// The <see cref="ActivatableHostfileEntry"/> class is the implementation
    /// of the <see cref="HostfileEntry"/> class and the <see cref="IActivatableHostfileEntry"/> interface.
    /// Instances of the <see cref="ActivatableHostfileEntry"/> class can be activated and deactivated.
    /// </summary>
    public class ActivatableHostfileEntry : HostfileEntry, IActivatableHostfileEntry
    {
        #region private fields

        /// <summary>A flag indicating whether this host is active.</summary>
        private bool isActive;

        #endregion

        #region constructor(s)

        /// <summary>Initializes a new instance of the <see cref="ActivatableHostfileEntry"/> class.</summary>
        /// <param name="entryType">The entry type.</param>
        /// <param name="parent">The parent <see cref="IHostfileEntry"/>.</param>
        /// <param name="name">The name of the enntry.</param>
        /// <param name="description">The description text.</param>
        /// <param name="childs">The childs.</param>
        /// <param name="propertyChangedCallBack">This event is fired whenever a property of this object changes.</param>
        protected ActivatableHostfileEntry(HostfileEntryType entryType, IHostfileEntry parent, string name, string description, HostfileEntryCollection childs, EventHandler propertyChangedCallBack)
            : base(entryType, parent, name, description, childs, propertyChangedCallBack)
        {
            this.IsActivatable = true;
        }

        #endregion

        #region Implementation of IActivatableHostfileEntry

        #region properties
        
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="HostfileEntry"/> is active or not.
        /// </summary>
        public bool IsActive
        {
            get
            {
                if (this.Childs != null && this.Childs.Activatable.ToList().Count > 0)
                {
                    bool allChildsAreActive = this.Childs.Activatable.OfType<ActivatableHostfileEntry>().All(activatableItem => activatableItem.IsActive);
                    return allChildsAreActive;
                }

                return this.isActive;
            }

            set
            {
                if (this.Childs != null && this.Childs.Activatable.ToList().Count > 0)
                {
                    foreach (ActivatableHostfileEntry activatableItem in this.Childs.Where(child => child.IsActivatable).OfType<ActivatableHostfileEntry>().Where(child => child.IsActive.Equals(value) == false))
                    {
                        activatableItem.IsActive = value;
                    }
                }
                else
                {
                    if (this.isActive.Equals(value) == false)
                    {
                        this.isActive = value;

                        this.OnPropertyChanged("IsActive");
                        this.OnPropertyChanged("ActiveRate");

                        if (this.Parent != null)
                        {
                            this.Parent.ThrowOnPropertyChangedEvent("IsActive");
                            this.Parent.ThrowOnPropertyChangedEvent("ActiveRate");
                        }                           
                    }                 
                }
            }
        }

        #endregion

        #region calculated properties

        /// <summary>Gets a double value representing the share of active child to non-active childs (e.g. 3/6 => 0.5).</summary>
        public double ActiveRate
        {
            get
            {
                if (this.Childs != null)
                {
                    double totalChildCount = this.Childs.Activatable.OfType<ActivatableHostfileEntry>().Count();
                    if (totalChildCount > 0)
                    {
                        double activeChildCount = this.Childs.Activatable.OfType<ActivatableHostfileEntry>().Count(activatableItem => activatableItem.IsActive);
                        double rate = activeChildCount / totalChildCount;

                        return rate;                        
                    }
                }

                return this.IsActive ? 1.0d : 0.0d;
            }
        }

        #endregion

        #region public methods

        /// <summary>Toggle the active state of this <see cref="IActivatableHostfileEntry"/>.</summary>
        public void ToggleActiveState()
        {
            bool previosState = this.IsActive;
            this.IsActive = !previosState;
        }

        #endregion

        #endregion
    }
}
