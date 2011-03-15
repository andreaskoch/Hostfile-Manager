namespace HostfileManager.Model
{
    using System;
    using System.Collections.Generic;

    using HostfileManager.Model.Base;

    /// <summary>
    /// The <see cref="Comment"/> class
    /// is a specialization of the generic <see cref="HostfileEntry"/> class and
    /// represents a text comment in a hosts file.
    /// </summary>
    public class Comment : HostfileEntry
    {
        #region constructor(s)

        /// <summary>Initializes a new instance of the <see cref="Comment"/> class.</summary>
        /// <param name="text">The commet text.</param>
        /// <param name="propertyChangedCallBack">This event is fired whenever a property of this object changes.</param>
        public Comment(string text, EventHandler propertyChangedCallBack)
            : base(HostfileEntryType.Comment, null, text, propertyChangedCallBack)
        {
        }

        #endregion

        #region factory methods

        /// <summary>Create an empty <see cref="Comment"/> with default settings.</summary>
        /// <param name="propertyChangedCallBack">This event is fired whenever a property of this object changes.</param>
        /// <returns>An empty <see cref="Comment"/> with default settings.</returns>
        public static Comment CreateEmpty(EventHandler propertyChangedCallBack)
        {
            return new Comment(App.GetApplicationResource<string>("CommentValue_Default"), propertyChangedCallBack);
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
            IEnumerable<string> lines = this.Name.Replace('\r', ' ').Split('\n');

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(text))
                {
                    text = string.Concat(Environment.NewLine, Constants.HostFileSyntaxCommentCharacter, " ", line);
                }
                else
                {
                    text += string.Concat(Environment.NewLine, Constants.HostFileSyntaxCommentCharacter, " ", line);
                }
            }

            return string.Concat(text, Environment.NewLine);
        }

        #endregion
    }
}
