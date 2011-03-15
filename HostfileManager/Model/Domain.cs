namespace HostfileManager.Model
{
    using System;

    using HostfileManager.Model.Base;

    /// <summary>
    /// The <see cref="Domain"/> class
    /// is a specialization of the generic <see cref="HostfileEntry"/> class and
    /// represents a single domain of a <see cref="Host"/> object (e.g. "example.com").
    /// </summary>
    public class Domain : HostfileEntry
    {
        #region constructor(s)

        /// <summary>Initializes a new instance of the <see cref="Domain"/> class.</summary>
        /// <param name="parent">The parent <see cref="Host"/> object.</param>
        /// <param name="domainName">The domain name.</param>
        /// <param name="propertyChangedCallBack">This event is fired whenever a property of this object changes.</param>
        public Domain(Host parent, string domainName, EventHandler propertyChangedCallBack)
            : base(HostfileEntryType.Domain, parent, domainName, propertyChangedCallBack)
        {
        }

        #endregion

        #region factory methods

        /// <summary>Create an empty <see cref="Domain"/> with default settings.</summary>
        /// <param name="parent">The parent <see cref="Host"/> object.</param>
        /// <param name="propertyChangedCallBack">This event is fired whenever a property of this object changes.</param>
        /// <returns>An empty <see cref="Domain"/> with default settings.</returns>
        public static Domain CreateEmpty(Host parent, EventHandler propertyChangedCallBack)
        {
            return new Domain(parent, App.GetApplicationResource<string>("DomainName_Default"), propertyChangedCallBack);
        }

        #endregion

        #region HostfileEntry overrides

        /// <summary>
        /// Get the text-representation of this object instance.
        /// </summary>
        /// <returns>The text-representation of the current object instance.</returns>
        public override string GetText()
        {
            return this.Name;
        }

        #endregion
    }
}
