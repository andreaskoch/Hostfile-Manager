namespace HostfileManager.Model.Base
{
    /// <summary>
    /// The <see cref="IActivatableHostfileEntry"/> interface offers 
    /// an "is-active" property and a method for toggling the the active state of an
    /// <see cref="IHostfileEntry"/> and  can be used as a specialization
    /// of the <see cref="IHostfileEntry"/> implementation.
    /// </summary>
    public interface IActivatableHostfileEntry
    {
        #region public properties

        /// <summary>Gets a value indicating whether this <see cref="HostfileEntry"/> is active or not (default = false).</summary>
        bool IsActive { get; }

        #endregion

        #region calculated properties

        /// <summary>Gets a double value representing the share of active child to non-active childs (e.g. 3/6 => 0.5).</summary>
        double ActiveRate { get; }

        #endregion

        #region public methods

        /// <summary>Toggle the active state of this <see cref="IActivatableHostfileEntry"/>.</summary>
        void ToggleActiveState();

        #endregion
    }
}
