namespace HostfileManager.Model
{
    using System;
    using System.Linq;

    using HostfileManager.Model.Base;

    /// <summary>
    /// The <see cref="HostGroup"/> class
    /// is a specialization of the generic <see cref="HostfileEntry"/> class
    /// and represents a collection of <see cref="Host"/> objects.
    /// </summary>
    public class HostGroup : ActivatableHostfileEntry
    {
        #region constructor(s)

        /// <summary>Initializes a new instance of the <see cref="HostGroup"/> class.</summary>
        /// <param name="groupName">The group name.</param>
        /// <param name="propertyChangedCallBack">This event is fired whenever a property of this object changes.</param>
        public HostGroup(string groupName, EventHandler propertyChangedCallBack)
            : base(HostfileEntryType.HostGroup, null, groupName, string.Empty, new HostfileEntryCollection(propertyChangedCallBack), propertyChangedCallBack)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="HostGroup"/> class.</summary>
        /// <param name="groupName">The group name.</param>
        /// <param name="groupDescription">The group description.</param>
        /// <param name="propertyChangedCallBack">This event is fired whenever a property of this object changes.</param>
        public HostGroup(string groupName, string groupDescription, EventHandler propertyChangedCallBack)
            : base(HostfileEntryType.HostGroup, null, groupName, groupDescription, new HostfileEntryCollection(propertyChangedCallBack), propertyChangedCallBack)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="HostGroup"/> class.</summary>
        /// <param name="groupName">The group name.</param>
        /// <param name="groupDescription">The group description.</param>
        /// <param name="childs">The childs.</param>
        /// <param name="propertyChangedCallBack">This event is fired whenever a property of this object changes.</param>
        public HostGroup(string groupName, string groupDescription, HostfileEntryCollection childs, EventHandler propertyChangedCallBack)
            : base(HostfileEntryType.HostGroup, null, groupName, groupDescription, childs, propertyChangedCallBack)
        {
        }

        #endregion

        #region factory methods

        /// <summary>Create an empty <see cref="HostGroup"/> with default settings.</summary>
        /// <param name="propertyChangedCallBack">This event is fired whenever a property of this object changes.</param>
        /// <returns>An empty <see cref="HostGroup"/> with default settings.</returns>
        public static HostGroup CreateEmpty(EventHandler propertyChangedCallBack)
        {
            return new HostGroup(App.GetApplicationResource<string>("HostsGroupName_Default"), propertyChangedCallBack);
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

            /* group description text */
            if (string.IsNullOrEmpty(this.Description) == false)
            {
                text += string.Concat(Environment.NewLine, Constants.HostFileSyntaxCommentCharacter, " ", this.Description);
            }
                
            /* group start */
            text += string.Concat(Environment.NewLine, Constants.HostFileSyntaxCommentCharacter, " ", Constants.HostFileSyntaxControlElementHostGroupStart, this.Name);

            /* group entries */
            foreach (Host h in this.Childs.OfType<Host>())
            {
                text = string.Concat(text, Environment.NewLine, h.GetText());
            }

            /* group end */
            text = string.Concat(text, Environment.NewLine, Constants.HostFileSyntaxCommentCharacter, " ", Constants.HostFileSyntaxControlElementHostGroupEnd, this.Name, Environment.NewLine);

            return text;
        }

        #endregion
    }
}
