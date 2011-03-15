namespace HostfileManager.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The <see cref="HostfileLine"/> class
    /// represents a single line in a hosts file and 
    /// offers various properties which help to determine the
    /// type and the context of that specific line.
    /// </summary>
    public class HostfileLine
    {
        #region private fields

        /// <summary>An instance of the parent <see cref="HostfileLineByLineNavigator"/> class.</summary>
        private readonly HostfileLineByLineNavigator lineByLineNavigator;

        /// <summary>The current line number (e.g. 13).</summary>
        private readonly int lineNumber;

        /// <summary>The text of this <see cref="HostfileLine"/> object (e.g. "127.0.0.1 localhost").</summary>
        private readonly string lineText;

        /// <summary>The information items / contents of this <see cref="HostfileLine"/> instance (e.g "127.0.0.1", "example.com").</summary>
        private List<string> values;

        /// <summary>A flag indicating whether the <see cref="isActive"/> flag has already been set (default = false).</summary>
        private bool isActiveHasBeenSet;

        /// <summary>A flag indicating whether this <see cref="HostfileLine"/> is active or commented out.</summary>
        private bool isActive;

        /// <summary>A flag indicating whether the <see cref="isActivatable"/> flag has already been set (default = false).</summary>
        private bool isActivatableHasBeenSet;

        /// <summary>A flag indicating whether this <see cref="HostfileLine"/> is activatable or not.</summary>
        private bool isActivatable;

        /// <summary>A flag indicating whether the <see cref="isHost"/> flag has already been set (default = false).</summary>
        private bool isHostHasBeenSet;

        /// <summary>A flag indicating whether this <see cref="HostfileLine"/> is a host (e.g. "127.0.0.1 localhost") or not.</summary>
        private bool isHost;

        /// <summary>A flag indicating whether the <see cref="isGroupStart"/> flag has already been set (default = false).</summary>
        private bool isGroupStartHasBeenSet;

        /// <summary>A flag indicating whether this <see cref="HostfileLine"/> is a group start element or not.</summary>
        private bool isGroupStart;

        /// <summary>A flag indicating whether the <see cref="isGroupEnd"/> flag has already been set (default = false).</summary>
        private bool isGroupEndHasBeenSet;

        /// <summary>A flag indicating whether this <see cref="HostfileLine"/> is a group end element or not.</summary>
        private bool isGroupEnd;

        /// <summary>A flag indicating whether the <see cref="isToggleModeOption"/> flag has already been set (default = false).</summary>
        private bool isToggleModeOptionHasBeenSet;

        /// <summary>A flag indicating whether this <see cref="HostfileLine"/> is a toggle-mode option or not.</summary>
        private bool isToggleModeOption;

        /// <summary>A flag indicating whether the <see cref="isControlElement"/> flag has already been set (default = false).</summary>
        private bool isControlElementHasBeenSet;

        /// <summary>A flag indicating whether this <see cref="HostfileLine"/> is some sort of control element (group start, group end, ...) or not.</summary>
        private bool isControlElement;

        /// <summary>A flag indicating whether the <see cref="isComment"/> flag has already been set (default = false).</summary>
        private bool isCommentHasBeenSet;

        /// <summary>A flag indicating whether this <see cref="HostfileLine"/> is a comment (e.g. "# Lorem Ipsum") or not.</summary>
        private bool isComment;

        /// <summary>A flag indicating whether the <see cref="isGlobalComment"/> flag has already been set (default = false).</summary>
        private bool isGlobalCommentHasBeenSet;

        /// <summary>A flag indicating whether this <see cref="HostfileLine"/> is a global comment (e.g. "# Lorem Ipsum") or not.</summary>
        private bool isGlobalComment;

        /// <summary>A flag indicating whether the <see cref="isMultiLineComment"/> flag has already been set (default = false).</summary>
        private bool isMultiLineCommentHasBeenSet;

        /// <summary>A flag indicating whether this <see cref="HostfileLine"/> is a comment which spreads more than one line or not.</summary>
        private bool isMultiLineComment;

        /// <summary>A flag indicating whether the <see cref="isEmpty"/> flag has already been set (default = false).</summary>
        private bool isEmptyHasBeenSet;

        /// <summary>A flag indicating whether this <see cref="HostfileLine"/> is an empty line or not.</summary>
        private bool isEmpty;

        /// <summary>A flag indicating whether the <see cref="isDescription"/> flag has already been set (default = false).</summary>
        private bool isDescriptionHasBeenSet;

        /// <summary>A flag indicating whether this <see cref="HostfileLine"/> is a description text or not.</summary>
        private bool isDescription;

        /// <summary>A flag indicating whether the <see cref="isMemberOfHostGroup"/> flag has already been set (default = false).</summary>
        private bool isMemberOfHostGroupHasBeenSet;

        /// <summary>A flag indicating whether this <see cref="HostfileLine"/> is a child of a group or not.</summary>
        private bool isMemberOfHostGroup;

        /// <summary>A flag indicating whether the <see cref="descriptionText"/> value has already been set (default = false).</summary>
        private bool descriptionTextHasBeenSet;

        /// <summary>A value representing the description text if this <see cref="HostfileLine"/> is a host description (<see cref="IsDescription"/>) and if the subsequent <see cref="HostfileLine"/> (<see cref="Next"/>) is a host (<see cref="IsHost"/>).</summary>
        private string descriptionText;

        /// <summary>A flag indicating whether the <see cref="parentHostGroup"/> value has already been set (default = false).</summary>
        private bool parentHostGroupHasBeenSet;

        /// <summary>A value representing the parent <see cref="HostfileLine"/> if this <see cref="HostfileLine"/> is a member of a group (<see cref="IsMemberOfHostGroup"/>).</summary>
        private HostfileLine parentHostGroup;

        /// <summary>A flag indicating whether the <see cref="groupName"/> value has already been set (default = false).</summary>
        private bool groupNameHasBeenSet;

        /// <summary>A value representing the name of the parent group if this <see cref="HostfileLine"/> is a member of a group (<see cref="IsMemberOfHostGroup"/>).</summary>
        private string groupName;

        /// <summary>A flag indicating whether the <see cref="multiLineCommentText"/> value has already been set (default = false).</summary>
        private bool multiLineCommentTextHasBeenSet;

        /// <summary>A value representing comment text of a multiline-comment if this <see cref="HostfileLine"/> is a part of a multiline comment (<see cref="IsMultiLineComment"/>).</summary>
        private string multiLineCommentText;

        /// <summary>A flag indicating whether the <see cref="multiLineCommentText"/> value has already been set (default = false).</summary>
        private bool multiLineCommentSizeHasBeenSet;

        /// <summary>A value representing number of lines of a multiline-comment if this <see cref="HostfileLine"/> is a part of a multiline comment (<see cref="IsMultiLineComment"/>).</summary>
        private int multiLineCommentSize;

        #endregion

        #region constructor(s)

        /// <summary>Initializes a new instance of the <see cref="HostfileLine"/> class.</summary>
        /// <param name="lineByLineNavigator">The line by line navigator.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="lineText">The line text.</param>
        public HostfileLine(HostfileLineByLineNavigator lineByLineNavigator, int lineNumber, string lineText)
        {
            this.lineByLineNavigator = lineByLineNavigator;
            this.lineNumber = lineNumber;
            this.lineText = lineText;
        }

        #endregion

        #region properties

        /// <summary>Gets the text of this <see cref="HostfileLine"/> object (e.g. "127.0.0.1 localhost").</summary>
        public string LineText
        {
            get
            {
                return this.lineText;
            }
        }

        /// <summary>Gets the current line number (e.g. 13) of this <see cref="HostfileLine"/>.</summary>
        public int LineNumber
        {
            get
            {
                return this.lineNumber;
            }
        }

        /// <summary>Gets an instance of the parent <see cref="HostfileLineByLineNavigator"/> class.</summary>
        public HostfileLineByLineNavigator LineByLineNavigator
        {
            get
            {
                return this.lineByLineNavigator;
            }
        }

        #endregion

        #region public properties

        /// <summary>Gets a value indicating whether this <see cref="HostfileLine"/> is a global comment (e.g. "# Lorem Ipsum") or not.</summary>
        public bool IsGlobalComment
        {
            get
            {
                if (this.isGlobalCommentHasBeenSet == false)
                {
                    this.isGlobalComment = this.IsComment && (this.Next == null || (this.Next.IsHost == false && this.Next.IsGroupStart == false));
                    this.isGlobalCommentHasBeenSet = true;
                }

                return this.isGlobalComment;                
            }
        }

        /// <summary>Gets a value indicating whether this <see cref="HostfileLine"/> is a comment which spreads more than one line or not.</summary>
        public bool IsMultiLineComment
        {
            get
            {
                if (this.isMultiLineCommentHasBeenSet == false)
                {
                    this.isMultiLineComment = this.IsGlobalComment && ((this.Next != null && this.Next.IsGlobalComment) || (this.Previous != null && this.Previous.IsGlobalComment));
                    this.isMultiLineCommentHasBeenSet = true;
                }

                return this.isMultiLineComment;
            }
        }

        /// <summary>Gets a value indicating whether this <see cref="HostfileLine"/> is a toggle-mode option or not.</summary>
        public bool IsToggleModeOption
        {
            get
            {
                if (this.isToggleModeOptionHasBeenSet == false)
                {
                    this.isToggleModeOption = (this.IsEmpty == false) && HostfileLineByLineNavigator.RegexFindToggleModeOption.IsMatch(this.LineText);
                    this.isToggleModeOptionHasBeenSet = true;
                }

                return this.isToggleModeOption;
            }
        }

        /// <summary>Gets a value indicating whether this <see cref="HostfileLine"/> is a host (e.g. "127.0.0.1 localhost") or not.</summary>
        public bool IsHost
        {
            get
            {
                if (this.isHostHasBeenSet == false)
                {
                    this.isHost = (this.IsEmpty == false) && HostfileLineByLineNavigator.RegexFindHost.IsMatch(this.LineText);
                    this.isHostHasBeenSet = true;
                }

                return this.isHost;
            }
        }

        /// <summary>Gets a value indicating whether this <see cref="HostfileLine"/> is a group start element or not.</summary>
        public bool IsGroupStart
        {
            get
            {
                if (this.isGroupStartHasBeenSet == false)
                {
                    this.isGroupStart = (this.IsEmpty == false) && HostfileLineByLineNavigator.RegexFindGroupStart.IsMatch(this.LineText);
                    this.isGroupStartHasBeenSet = true;
                }

                return this.isGroupStart;
            }            
        }

        /// <summary>Gets a value indicating whether this <see cref="HostfileLine"/> is a child of a group or not.</summary>
        public bool IsMemberOfHostGroup
        {
            get
            {
                if (this.isMemberOfHostGroupHasBeenSet == false)
                {
                    this.isMemberOfHostGroup = this.ParentHostGroup != null;
                    this.isMemberOfHostGroupHasBeenSet = true;
                }

                return this.isMemberOfHostGroup;                
            }
        }

        /// <summary>Gets a value indicating whether this <see cref="HostfileLine"/> is activatable or not.</summary>
        public bool IsActivatable
        {
            get
            {
                if (this.isActivatableHasBeenSet == false)
                {
                    this.isActivatable = this.IsHost;
                    this.isActivatableHasBeenSet = true;
                }

                return this.isActivatable;
            }
        }

        /// <summary>Gets a value indicating whether this <see cref="HostfileLine"/> is active or commented out.</summary>
        public bool IsActive
        {
            get
            {
                if (this.isActiveHasBeenSet == false)
                {
                    this.isActive = this.IsActivatable && HostfileLineByLineNavigator.RegexIsCommentLine.IsMatch(this.LineText) == false;
                    this.isActiveHasBeenSet = true;
                }

                return this.isActive;
            }
        }

        /// <summary>Gets the parent <see cref="HostfileLine"/> if this <see cref="HostfileLine"/> is a member of a group (<see cref="IsMemberOfHostGroup"/>).</summary>
        public HostfileLine ParentHostGroup
        {
            get
            {
                if (this.parentHostGroupHasBeenSet == false)
                {
                    this.parentHostGroup = GetParentHostGroup(this);
                    this.parentHostGroupHasBeenSet = true;
                }

                return this.parentHostGroup;
            }
        }

        /// <summary>Gets the information items / contents of this <see cref="HostfileLine"/> instance (e.g "127.0.0.1", "example.com").</summary>
        public List<string> Values
        {
            get
            {
                if (this.values == null)
                {
                    this.values = new List<string>();

                    /* 
                     * extract values from the specified text
                     * using the respective regular expression
                     */
                    if (this.IsGlobalComment || this.IsDescription)
                    {
                        this.values = GetValues(HostfileLineByLineNavigator.RegexIsCommentLine, this.LineText);
                    }
                    else if (this.IsGroupStart)
                    {
                        this.values = GetValues(HostfileLineByLineNavigator.RegexFindGroupStart, this.LineText);
                    }
                    else if (this.IsGroupEnd)
                    {
                        this.values = GetValues(HostfileLineByLineNavigator.RegexFindGroupEnd, this.LineText);
                    }
                    else if (this.IsHost)
                    {
                        this.values = GetValues(HostfileLineByLineNavigator.RegexFindHost, this.LineText);
                    }
                    else if (this.IsToggleModeOption)
                    {
                        this.values = GetValues(HostfileLineByLineNavigator.RegexFindToggleModeOption, this.LineText);
                    }
                }

                return this.values;
            }
        }

        /// <summary>Gets the host description if this <see cref="HostfileLine"/> is a description (<see cref="IsDescription"/>) and if the subsequent <see cref="HostfileLine"/> (<see cref="Next"/>) is a host (<see cref="IsHost"/>) or a group-start (<see cref="IsGroupStart"/>).</summary>
        public string DescriptionText
        {
            get
            {
                if (this.descriptionTextHasBeenSet == false)
                {
                    if (this.Previous != null && this.Previous.IsDescription)
                    {
                        this.descriptionText = this.Previous.Values.FirstOrDefault();
                    }

                    this.descriptionTextHasBeenSet = true;
                }

                return this.descriptionText;
            }
        }

        /// <summary>Gets the comment text if this <see cref="HostfileLine"/> is a part of a multiline comment (<see cref="IsMultiLineComment"/>).</summary>
        public string MultiLineCommentText
        {
            get
            {
                if (this.multiLineCommentTextHasBeenSet == false)
                {
                    this.multiLineCommentText = this.IsMultiLineComment ? this.GetMultiLineCommentText(this) : null;
                    this.multiLineCommentTextHasBeenSet = true;
                }

                return this.multiLineCommentText;
            }
        }

        /// <summary>Gets the number of lines of a multiline-comment if this <see cref="HostfileLine"/> is a part of a multiline comment (<see cref="IsMultiLineComment"/>).</summary>
        public int MultiLineCommentSize
        {
            get
            {
                if (this.multiLineCommentSizeHasBeenSet == false)
                {
                    List<HostfileLine> comments = this.GetAllRelatedComments(this);
                    this.multiLineCommentSize = comments != null ? comments.Count : 0;
                    this.multiLineCommentSizeHasBeenSet = true;
                }

                return this.multiLineCommentSize;
            }
        }

        /// <summary>Gets the name of the parent group if this <see cref="HostfileLine"/> is a member of a group (<see cref="IsMemberOfHostGroup"/>).</summary>
        public string GroupName
        {
            get
            {
                if (this.groupNameHasBeenSet == false)
                {
                    this.groupName = (this.ParentHostGroup != null) ? this.ParentHostGroup.Values.FirstOrDefault() : null;
                    this.groupNameHasBeenSet = true;
                }

                return this.groupName;
            }
        }

        #endregion

        #region protected properties

        /// <summary>Gets the previous <see cref="HostfileLine"/> from the <see cref="HostfileLineByLineNavigator"/> if there is one; otherwise null.</summary>
        protected HostfileLine Previous
        {
            get
            {
                int previousLineNumber = this.LineNumber - 1;
                if (this.LineByLineNavigator != null && previousLineNumber > 0 && previousLineNumber <= this.LineByLineNavigator.Count)
                {
                    return this.LineByLineNavigator[previousLineNumber];
                }

                return null;
            }
        }

        /// <summary>Gets the next <see cref="HostfileLine"/> from the <see cref="HostfileLineByLineNavigator"/> if there is one; otherwise null.</summary>
        protected HostfileLine Next
        {
            get
            {
                int nextLineNumber = this.LineNumber + 1;
                if (this.LineByLineNavigator != null && nextLineNumber > 0 && nextLineNumber <= this.LineByLineNavigator.Count)
                {
                    return this.LineByLineNavigator[nextLineNumber];
                }

                return null;
            }
        }

        /// <summary>Gets a value indicating whether this <see cref="HostfileLine"/> is an empty line or not.</summary>
        protected bool IsEmpty
        {
            get
            {
                if (this.isEmptyHasBeenSet == false)
                {
                    this.isEmpty = string.IsNullOrEmpty(this.LineText);
                    this.isEmptyHasBeenSet = true;
                }

                return this.isEmpty;
            }
        }

        /// <summary>Gets a value indicating whether this <see cref="HostfileLine"/> is some sort of control element (group start, group end, ...) or not.</summary>
        protected bool IsControlElement
        {
            get
            {
                if (this.isControlElementHasBeenSet == false)
                {
                    this.isControlElement = this.IsHost || this.IsGroupStart || this.IsGroupEnd || this.IsToggleModeOption;
                    this.isControlElementHasBeenSet = true;
                }

                return this.isControlElement;
            }
        }

        /// <summary>Gets a value indicating whether this <see cref="HostfileLine"/> is a comment (e.g. "# Lorem Ipsum") or not.</summary>
        protected bool IsComment
        {
            get
            {
                if (this.isCommentHasBeenSet == false)
                {
                    this.isComment = (this.IsControlElement == false) && HostfileLineByLineNavigator.RegexIsCommentLine.IsMatch(this.LineText);
                    this.isCommentHasBeenSet = true;
                }

                return this.isComment;
            }
        }

        /// <summary>Gets a value indicating whether this <see cref="HostfileLine"/> is a description text or not.</summary>
        protected bool IsDescription
        {
            get
            {
                if (this.isDescriptionHasBeenSet == false)
                {
                    this.isDescription = this.IsComment && (this.Next != null && (this.Next.IsHost || this.Next.IsGroupStart));
                    this.isDescriptionHasBeenSet = true;
                }

                return this.isDescription;
            }
        }

        /// <summary>Gets a value indicating whether this <see cref="HostfileLine"/> is a group end element or not.</summary>
        protected bool IsGroupEnd
        {
            get
            {
                if (this.isGroupEndHasBeenSet == false)
                {
                    this.isGroupEnd = (this.IsEmpty == false) && HostfileLineByLineNavigator.RegexFindGroupEnd.IsMatch(this.LineText);
                    this.isGroupEndHasBeenSet = true;
                }

                return this.isGroupEnd;
            }
        }

        #endregion

        #region object overrides

        /// <summary>Returns a string that represents the current <see cref="object"/>.</summary>
        /// <returns>A string that represents the current <see cref="object"/>.</returns>
        public override string ToString()
        {
            return string.Format("{0} #{1}: {2}", "HostfileLine", this.LineNumber, string.IsNullOrEmpty(this.LineText) ? " - empty -" : this.LineText);
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
                var otherObj = obj as HostfileLine;
                if (otherObj != null)
                {
                    return this.LineText.Equals(otherObj.LineText);
                }
            }

            return false;
        }

        #endregion

        #region private static functions

        /// <summary>Returns a list of all values contained in the specified <paramref name="text"/>.</summary>
        /// <param name="regex">The regular expression which contains the groups that shall be used as the "values" that this function returns.</param>
        /// <param name="text">The text of a <see cref="HostfileLine"/>.</param>
        /// <returns>A <see cref="List{String}"/> of all values contained in the specified <paramref name="text"/>.</returns>
        private static List<string> GetValues(Regex regex, string text)
        {
            List<string> regexGroupValues = new List<string>();

            if (regex != null && string.IsNullOrEmpty(text) == false)
            {
                MatchCollection matches = regex.Matches(text);
                if (matches.Count > 0)
                {
                    Match firstMatch = matches[0];
                    if (firstMatch.Success && firstMatch.Groups.Count > 1)
                    {
                        for (int i = 1; i < firstMatch.Groups.Count; i++)
                        {
                            regexGroupValues.Add(firstMatch.Groups[i].Value.Trim());
                        }
                    }
                }
            }

            return regexGroupValues;
        }

        /// <summary>Return the parent <see cref="HostfileLine"/> which marks the group-start for the specified <paramref name="line"/>; if the specified <paramref name="line"/> is part of a group.</summary>
        /// <param name="line">The <see cref="HostfileLine"/> object that is part of group.</param>
        /// <returns>The parent <see cref="HostfileLine"/> which marks the group-start for the specified <paramref name="line"/>; if the specified <paramref name="line"/> is part of a group. Otherwise null.</returns>
        private static HostfileLine GetParentHostGroup(HostfileLine line)
        {
            bool isAGroupControlElement = line.IsGroupStart || line.IsGroupEnd;

            if (isAGroupControlElement == false)
            {
                while ((line = line.Previous) != null)
                {
                    if (line.IsGroupEnd)
                    {
                        return null;
                    }

                    if (line.IsGroupStart)
                    {
                        return line;
                    }
                }
            }
            else
            {
                return line;
            }

            return null;
        }

        #endregion

        #region private functions

        /// <summary>Returns a list of all related <see cref="HostfileLine"/> objects that are part of the same multiline comment; if the specified <paramref name="line"/> is a multiline comment.</summary>
        /// <param name="line">The <see cref="HostfileLine"/> which is a member of a multiline comment.</param>
        /// <returns>A <see cref="List{HostfileLine}"/> of all related <see cref="HostfileLine"/> objects that are part of the same multiline comment; if the specified <paramref name="line"/> is a multiline comment.</returns>
        private List<HostfileLine> GetAllRelatedComments(HostfileLine line)
        {
            if (line == null)
            {
                return null;
            }

            /* navigate to the beginning of the multi-line comment */
            while (line.Previous != null && line.Previous.IsMultiLineComment)
            {
                line = line.Previous;
            }

            int currentLineNumber = line.LineNumber;
            int nextNonMultiLineCommentLineNumber = this.LineByLineNavigator.Count;
            HostfileLine nextNonMultiLineComment = this.LineByLineNavigator.Where(l => l.LineNumber > currentLineNumber && l.IsMultiLineComment == false).FirstOrDefault();
            if (nextNonMultiLineComment != null)
            {
                nextNonMultiLineCommentLineNumber = nextNonMultiLineComment.LineNumber;
            }

            List<HostfileLine> comments =
                this.LineByLineNavigator.Where(
                    l =>
                    l.LineNumber >= currentLineNumber && l.LineNumber < nextNonMultiLineCommentLineNumber &&
                    l.IsMultiLineComment).ToList();

            return comments;
        }

        /// <summary>Returns the text of a multiline comment if the specified <paramref name="line"/> is part of a multiline comment.</summary>
        /// <param name="line">The <see cref="HostfileLine"/> which is a member of a multiline comment.</param>
        /// <returns>The text of a multiline comment if the specified <paramref name="line"/> is part of a multiline comment; otherwise null.</returns>
        private string GetMultiLineCommentText(HostfileLine line)
        {
            List<HostfileLine> relatedComments = this.GetAllRelatedComments(line);
            if (relatedComments != null && relatedComments.Count > 0)
            {
                return relatedComments.Select(l => l.Values.FirstOrDefault()).Aggregate((l1, l2) => string.Concat(l1, Environment.NewLine, l2));
            }

            return null;
        }

        #endregion
    }
}