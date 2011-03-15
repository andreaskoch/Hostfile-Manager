namespace HostfileManager.Model.Base
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// The <see cref="IHostfileEntry"/> interface provides
    /// general properties and functions for all possible entries of a hosts file.
    /// </summary>
    public interface IHostfileEntry
    {
        #region events

        /// <summary>This event is fired whenever a property of this object changes.</summary>
        event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region public properties

        /// <summary>Gets the type of this <see cref="HostfileEntry"/>.</summary>
        HostfileEntryType EntryType { get; }

        /// <summary>Gets the name of this <see cref="HostfileEntry"/>.</summary>
        string Name { get; }

        /// <summary>Gets the description text of this <see cref="HostfileEntry"/>.</summary>
        string Description { get; }

        /// <summary>Gets the childs of this <see cref="HostfileEntry"/>.</summary>
        HostfileEntryCollection Childs { get;  }

        /// <summary>Gets a unique identifier for this <see cref="HostfileEntry"/>.</summary>
        Guid UniqueIdentifier { get;  }

        /// <summary>Gets a unique string signature for this <see cref="HostfileEntry"/>.</summary>
        string Signature { get; }

        /// <summary>Gets the parent <see cref="HostfileEntry"/> object.</summary>
        IHostfileEntry Parent { get;  }

        /// <summary>Gets a value indicating whether this <see cref="HostfileEntry"/> is activatable (default = false).</summary>
        bool IsActivatable { get; }

        /// <summary>Gets a value indicating whether this <see cref="IHostfileEntry"/> has a parent object or not</summary>
        bool HasParent { get; }

        #endregion

        #region public functions

        /// <summary>Add a child to this <see cref="IHostfileEntry"/>.</summary>
        /// <param name="child">The child entry.</param>
        /// <returns>True if the specified <paramref name="child"/> was added to the list of childs; otherwise false.</returns>
        bool AddChild(IHostfileEntry child);

        /// <summary>Add a child to the top of this <see cref="HostFile"/> instance.</summary>
        /// <param name="child">The child entry.</param>
        /// <returns>True if the specified <paramref name="child"/> was added to the list of childs; otherwise false.</returns>
        bool AddChildToTop(IHostfileEntry child);

        /// <summary>Delete this <see cref="IHostfileEntry"/>.</summary>
        /// <returns>True if this <see cref="IHostfileEntry"/> was successfully deleted; otherwise false.</returns>
        bool Delete();

        /// <summary>Delete the specified <see cref="IHostfileEntry"/>.</summary>
        /// <param name="hostfileEntry">A <see cref="IHostfileEntry"/>.</param>
        /// <returns>True if the specified <see cref="IHostfileEntry"/> was successfully deleted; otherwise false.</returns>
        bool DeleteChild(IHostfileEntry hostfileEntry);

        /// <summary>Move the <see cref="IHostfileEntry"/> up.</summary>
        /// <returns>True if this <see cref="IHostfileEntry"/> was moved up successfully; otherwise false.</returns>
        bool MoveUp();

        /// <summary>Move the <see cref="IHostfileEntry"/> down.</summary>
        /// <returns>True if this <see cref="IHostfileEntry"/> was moved down successfully; otherwise false.</returns>
        bool MoveDown();

        /// <summary>Move the child with the specifid <paramref name="hostfileEntryIdentifier"/> up.</summary>
        /// <param name="hostfileEntryIdentifier">The unique identifier of the <see cref="IHostfileEntry"/> to shall be moved up.</param>
        /// <returns>True if the <see cref="IHostfileEntry"/> with the specified <paramref name="hostfileEntryIdentifier"/> was moved up successfully; otherwise false.</returns>
        bool MoveChildUp(Guid hostfileEntryIdentifier);

        /// <summary>Move the child with the specifid <paramref name="hostfileEntryIdentifier"/> down.</summary>
        /// <param name="hostfileEntryIdentifier">The unique identifier of the <see cref="IHostfileEntry"/> to shall be moved down.</param>
        /// <returns>True if the <see cref="IHostfileEntry"/> with the specified <paramref name="hostfileEntryIdentifier"/> was moved down successfully; otherwise false.</returns>
        bool MoveChildDown(Guid hostfileEntryIdentifier);

        /// <summary>
        /// Get the text-representation of the current object.
        /// </summary>
        /// <returns>The text-representation of the current object instance.</returns>
        string GetText();

        /// <summary>Throw the OnPropertyChanged event.</summary>
        /// <param name="propertyName">The property name.</param>
        void ThrowOnPropertyChangedEvent(string propertyName);

        #endregion
    }
}
