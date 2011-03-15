namespace HostfileManager.Model
{
    using System.Globalization;
    using System.Xml.Serialization;

    /// <summary>
    /// The <see cref="ApplicationSettings"/> class encapsulates
    /// all properties of the current application that can be stored
    /// outside the application for future reuse.
    /// </summary>
    [XmlRoot(ElementName = "ApplicationSettings", Namespace = "http://www.w3.org/2005/HostfileManager")]
    public class ApplicationSettings
    {
        #region properties

        /// <summary>
        /// Gets or sets the application-width property of this <see cref="ApplicationSettings"/> object.
        /// </summary>
        [XmlElement(ElementName = "Width")]
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the application-height property of this <see cref="ApplicationSettings"/> object.
        /// </summary>
        [XmlElement(ElementName = "Height")]
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets the application top-position property of this <see cref="ApplicationSettings"/> object.
        /// </summary>
        [XmlElement(ElementName = "Top")]
        public double Top { get; set; }

        /// <summary>
        /// Gets or sets the application top-left-position property of this <see cref="ApplicationSettings"/> object.
        /// </summary>
        [XmlElement(ElementName = "Left")]
        public double Left { get; set; }

        /// <summary>
        /// Gets or sets the application view-mode property (e.g. <see cref="ViewMode.Editor"/>, <see cref="ViewMode.Overview"/>, <see cref="ViewMode.TextEditor"/>) of this <see cref="ApplicationSettings"/> object.
        /// </summary>
        [XmlElement(ElementName = "LastViewMode")]
        public ViewMode LastViewMode { get; set; }

        /// <summary>Gets or sets the application culture code (e.g. "en-US", "de-DE").</summary>
        [XmlElement(ElementName = "CultureCode")]
        public string CultureCode { get; set; }

        /// <summary>Gets or sets a value indicating whether WPF hardware acceleration is disabled (default = false).
        /// Note: Hardware acceleration might cause problems on some VMWare machine.
        /// </summary>
        [XmlElement(ElementName = "DisableWpfHardwareAcceleration")]
        public bool DisableWpfHardwareAcceleration { get; set; }

        #endregion

        #region factory methods

        /// <summary>Create a new <see cref="ApplicationSettings"/> object.</summary>
        /// <param name="width">The width of the application-window.</param>
        /// <param name="height">The height of the application-window.</param>
        /// <param name="topPosition">The top-position of the application-window.</param>
        /// <param name="leftPosition">The left-position of the application-window.</param>
        /// <param name="lastViewMode">The view-mode of the application-window.</param>
        /// <param name="culture">The application culture.</param>
        /// <param name="disableWpfHardwareAcceleration">A flag indicating whether WPF hardware acceleration is disabled (default = false).</param>
        /// <returns>An initialized <see cref="ApplicationSettings"/> object.</returns>
        public static ApplicationSettings CreateApplicationSettings(double width, double height, double topPosition, double leftPosition, ViewMode lastViewMode, CultureInfo culture, bool disableWpfHardwareAcceleration)
        {
            return new ApplicationSettings { Width = width, Height = height, Top = topPosition, Left = leftPosition, LastViewMode = lastViewMode, CultureCode = culture.ToString(), DisableWpfHardwareAcceleration = disableWpfHardwareAcceleration };
        }

        #endregion

        #region object overrides

        /// <summary>
        /// Returns a String that represents the current <see cref="object"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents the current <see cref="object"/>.</returns>
        public override string ToString()
        {
            return string.Format("{0} (Width: {1}, Height: {2}, TopPosition: {3}, LeftPosition: {4}, LastViewMode: {5}, CultureCode: {6}, DisableWpfHardwareAcceleration: {7})", "ApplicationSettings", this.Width, this.Height, this.Top, this.Left, this.LastViewMode, this.CultureCode, this.DisableWpfHardwareAcceleration);
        }

        #endregion
    }
}
