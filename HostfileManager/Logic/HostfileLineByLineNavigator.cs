namespace HostfileManager.Logic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Model;

    /// <summary>
    /// The <see cref="HostfileLineByLineNavigator"/> class is a container
    /// element for all <see cref="HostfileLine"/> objecta in a hosts file.
    /// </summary>
    public class HostfileLineByLineNavigator : IEnumerable<HostfileLine>
    {
        #region public fields

        /// <summary>A list of all <see cref="RegexOptions"/> used for regular expressions in the <see cref="HostfileLineByLineNavigator"/> class.</summary>
        public static readonly RegexOptions RegexOptionList = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled;

        /// <summary>
        /// Regular expression for identifying lines which are toggle-mode options.
        /// </summary>
        public static readonly Regex RegexFindToggleModeOption = new Regex(string.Concat(Constants.HostFileSyntaxCommentCharacter, " ", Constants.HostFileSyntaxControlElemenExclusiveToggleMode, "(?<", Constants.RegularExpressionParameterGroupName1, ">.+)"), RegexOptionList);

        /// <summary>
        /// Regular expression for identifying lines which start a host-group.
        /// </summary>
        public static readonly Regex RegexFindGroupStart = new Regex(string.Concat(Constants.HostFileSyntaxCommentCharacter, " ", Constants.HostFileSyntaxControlElementHostGroupStart, "(?<", Constants.RegularExpressionParameterGroupName1, ">[^#]+)"), RegexOptionList);

        /// <summary>
        /// Regular expression for identifying lines which end a host-group.
        /// </summary>
        public static readonly Regex RegexFindGroupEnd = new Regex(string.Concat(Constants.HostFileSyntaxCommentCharacter, " ", Constants.HostFileSyntaxControlElementHostGroupEnd, "(?<", Constants.RegularExpressionParameterGroupName1, ">[^#]+)"), RegexOptionList);

        /// <summary>
        /// A regular expression for identifying comments.
        /// </summary>
        public static readonly Regex RegexIsCommentLine = new Regex(string.Concat(@"\s*\", Constants.HostFileSyntaxCommentCharacter, "(?<", Constants.RegularExpressionParameterGroupName1, ">.*)"), RegexOptionList);

        /// <summary>
        /// A regular expression for parsing the ip-address and the domain names from a hosts file entry.
        /// </summary>
        public static readonly Regex RegexFindHost = new Regex(string.Concat(@"(?<", Constants.RegularExpressionParameterGroupName1, @">[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+)[\s\t]+?(?<", Constants.RegularExpressionParameterGroupName2, @">[a-z0-9\.\-\s]{2,})"), RegexOptionList);

        #endregion

        #region private fields

        /// <summary>The lines of the hostfile.</summary>
        private readonly List<HostfileLine> lines;

        #endregion

        #region constructor(s)

        /// <summary>Initializes a new instance of the <see cref="HostfileLineByLineNavigator"/> class.</summary>
        /// <param name="lines">A list of text lines.</param>
        public HostfileLineByLineNavigator(IEnumerable<string> lines)
        {
            this.lines = lines.Select((line, index) => new HostfileLine(this, index + 1, line)).ToList();
        }

        #endregion

        #region properties

        /// <summary>Gets the number of <see cref="HostfileLine"/> object in this <see cref="HostfileLineByLineNavigator"/> instance.</summary>
        public int Count
        {
            get { return this.lines.Count; }
        }

        #endregion

        #region indexer

        /// <summary>Gets the <see cref="HostfileLine"/> object with specified <paramref name="lineNumber"/> (starting with line 1).</summary>
        /// <param name="lineNumber">The line number (starting at 1).</param>
        /// <exception cref="ArgumentOutOfRangeException">If the specified <paramref name="lineNumber"/> does not exist in the current <see cref="HostfileLineByLineNavigator"/> instance.</exception>
        public HostfileLine this[int lineNumber]
        {
            get { return this.GetElementAt(lineNumber); }
        }

        #endregion

        #region public functions

        /// <summary>Gets the line number (starting at line 1) of the specified <see cref="HostfileLine"/> object.</summary>
        /// <param name="line">The <see cref="HostfileLine"/> object.</param>
        /// <returns>The line number (starting at line 1) of the specified <see cref="HostfileLine"/> object. -1 if the specified <paramref name="line"/> was not found.</returns>
        public int IndexOf(HostfileLine line)
        {
            if (line != null)
            {
                for (var i = 0; i < this.Count; i++)
                {
                    if (this[i].Equals(line))
                    {
                        return i + 1;
                    }
                }
            }

            return -1;
        }

        /// <summary>Gets the <see cref="HostfileLine"/> object with specified <paramref name="lineNumber"/> (starting with line 1).</summary>
        /// <param name="lineNumber">The line number (starting at 1).</param>
        /// <returns>The <see cref="HostfileLine"/> object with specified <paramref name="lineNumber"/> (starting with line 1).</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the specified <paramref name="lineNumber"/> does not exist in the current <see cref="HostfileLineByLineNavigator"/> instance.</exception>
        public HostfileLine GetElementAt(int lineNumber)
        {
            if (lineNumber < 1 || lineNumber > this.lines.Count)
            {
                throw new ArgumentOutOfRangeException("lineNumber");
            }

            return this.lines[lineNumber - 1];
        }

        #endregion

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<HostfileLine> GetEnumerator()
        {
            for (int lineNumber = 1; lineNumber <= this.Count; lineNumber++)
            {
                yield return this[lineNumber];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
