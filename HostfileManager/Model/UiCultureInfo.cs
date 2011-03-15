namespace HostfileManager.Model
{
    using System.Globalization;

    using HostfileManager.Logic;

    /// <summary>
    /// The <see cref="UiCultureInfo"/> class is an extension
    /// of the regular <see cref="CultureInfo"/> class with some
    /// additional properties.
    /// </summary>
    public class UiCultureInfo : CultureInfo
    {
        #region constructor(s)

        /// <summary>Initializes a new instance of the <see cref="UiCultureInfo"/> class.</summary>
        /// <param name="cultureInfo">The culture info.</param>
        public UiCultureInfo(CultureInfo cultureInfo)
            : base(cultureInfo.ToString())
        {
        }

        /// <summary>Initializes a new instance of the <see cref="UiCultureInfo"/> class.</summary>
        /// <param name="name">The culture name.</param>
        public UiCultureInfo(string name)
            : base(name)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="UiCultureInfo"/> class.</summary>
        /// <param name="name">The culture name.</param>
        /// <param name="useUserOverride">The use user override.</param>
        public UiCultureInfo(string name, bool useUserOverride)
            : base(name, useUserOverride)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="UiCultureInfo"/> class.</summary>
        /// <param name="culture">The culture.</param>
        public UiCultureInfo(int culture)
            : base(culture)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="UiCultureInfo"/> class.</summary>
        /// <param name="culture">The culture.</param>
        /// <param name="useUserOverride">The use user override.</param>
        public UiCultureInfo(int culture, bool useUserOverride)
            : base(culture, useUserOverride)
        {
        }

        #endregion

        #region properties

        /// <summary>Gets a value indicating whether the current <see cref="UiCultureInfo"/> object instance is the current user-interface culture.</summary>
        public bool IsActiveLanguage
        {
            get
            {
                return this.Equals(TranslationManager.Instance.CurrentCulture);
            }
        }

        #endregion
    }
}
