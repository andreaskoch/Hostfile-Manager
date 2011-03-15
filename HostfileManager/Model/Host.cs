namespace HostfileManager.Model
{
    using System;
    using System.Linq;

    using HostfileManager.Model.Base;

    /// <summary>
    /// The <see cref="Host"/> class
    /// is a specialization of the generic <see cref="HostfileEntry"/> class and represents
    /// a host in a hosts file (e.g. "127.0.0.1  localhost").
    /// </summary>
    public class Host : ActivatableHostfileEntry
    {
        #region constructor(s)

        /// <summary>Initializes a new instance of the <see cref="Host"/> class.</summary>
        /// <param name="parent">The parent <see cref="HostGroup"/> object.</param>
        /// <param name="hostname">The hostname.</param>
        /// <param name="description">A description text.</param>
        /// <param name="propertyChangedCallBack">This event is fired whenever a property of this object changes.</param>
        public Host(HostGroup parent, string hostname, string description, EventHandler propertyChangedCallBack)
            : base(HostfileEntryType.Host, parent, hostname, description, new HostfileEntryCollection(propertyChangedCallBack), propertyChangedCallBack)
        {
        }

        #endregion

        #region factory methods

        /// <summary>Create an empty <see cref="Host"/> with default settings.</summary>
        /// <param name="parent">The parent <see cref="HostGroup"/>.</param>
        /// <param name="propertyChangedCallBack">This event is fired whenever a property of this object changes.</param>
        /// <returns>An empty <see cref="Host"/> with default settings.</returns>
        public static Host CreateEmpty(HostGroup parent, EventHandler propertyChangedCallBack)
        {
            Host host = new Host(parent, App.GetApplicationResource<string>("HostIp_Default"), string.Empty, propertyChangedCallBack);
            host.Childs.Add(Domain.CreateEmpty(host, propertyChangedCallBack));
            return host;
        }

        #endregion

        #region HostfileEntry overrides

        /// <summary>
        /// Get the text-representation of this object instance.
        /// </summary>
        /// <returns>The text-representation of the current object instance.</returns>
        public override string GetText()
        {
            string text = string.Empty;

            /* host description */
            if (string.IsNullOrEmpty(this.Description) == false)
            {
                text += string.Concat(Environment.NewLine, Constants.HostFileSyntaxCommentCharacter, " ", this.Description, Environment.NewLine);
            }

            /* active? */
            if (this.IsActive == false)
            {
                text += string.Concat(Constants.HostFileSyntaxCommentCharacter, " ");
            }

            /* ip address */
            text += string.Concat(this.Name, "\t");

            /* domains */
            foreach (Domain domain in this.Childs.OfType<Domain>())
            {
                text += string.Concat(domain.Name, " ");
            }

            return text;
        }

        #endregion
    }
}
