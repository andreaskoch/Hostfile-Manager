namespace HostfileManager.Model
{
    using System;

    /// <summary>
    /// The <see cref="ViewModeEventArgs"/> calls implements the <see cref="EventArgs"/> class
    /// and can be  used a an event argugment for global application user interface events.
    /// </summary>
    public class ViewModeEventArgs : EventArgs
    {
        #region constructor(s)

        /// <summary>Initializes a new instance of the <see cref="ViewModeEventArgs"/> class.</summary>
        /// <param name="sourceView">The source view.</param>
        /// <param name="targetPath">The target path.</param>
        public ViewModeEventArgs(ViewMode sourceView, string targetPath)
        {
            this.SourceView = sourceView;
            this.TargetPath = targetPath;
        }

        #endregion

        #region properties

        /// <summary>Gets the <see cref="ViewMode"/> of the event source.</summary>
        public ViewMode SourceView { get; private set; }

        /// <summary>Gets current target path of the event source.</summary>
        public string TargetPath { get; private set; }

        #endregion
    }
}
