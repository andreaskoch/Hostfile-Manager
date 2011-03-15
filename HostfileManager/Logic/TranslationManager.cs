namespace HostfileManager.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Windows;

    using Model;

    /// <summary>
    /// The <see cref="TranslationManager"/> class provides
    /// localization-functions and properties.
    /// </summary>
    public class TranslationManager
    {
        #region Constants and Fields

        /// <summary>The <see cref="TranslationManager"/> manager instance.</summary>
        private static TranslationManager translationManager;

        #endregion

        #region constructor(s)

        /// <summary>Prevents a default instance of the <see cref="TranslationManager"/> class from being created.</summary>
        /// <exception cref="ApplicationException"></exception>
        private TranslationManager()
        {
            if (this.Languages.Count == 0)
            {
                throw new ApplicationException("Cannot initialize the TranslationManager without at least one default language.");
            }

            /* select a supported language */
            if (this.IsSupportedLanguage(this.CurrentCulture) == false)
            {
                this.CurrentCulture = this.Languages.First();
            }

            /* load localization dictionary */
            LoadTranslationDictionary(this.CurrentCulture);
        }

        #endregion

        #region singleton accessor

        /// <summary>Gets an instance of the <see cref="TranslationManager"/> class.</summary>
        public static TranslationManager Instance
        {
            get
            {
                return translationManager ?? (translationManager = new TranslationManager());
            }
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the available languages (default = "en-US").
        /// </summary>
        /// <value>The available languages (default = "en-US").</value>
        public List<CultureInfo> Languages
        {
            get
            {
                char seperator = App.GetApplicationResource("ApplicationLanguagesSeperator", ',');
                string languagesString = App.GetApplicationResource<string>("ApplicationLanguages");

                if (string.IsNullOrEmpty(languagesString) == false && string.IsNullOrEmpty(seperator.ToString()) == false)
                {
                    return languagesString.Split(seperator).Where(culture => string.IsNullOrEmpty(culture) == false).Select(cultureCode => new CultureInfo(cultureCode)).ToList();
                }

                /* default culture */
                string defaultLanguageCode = App.GetApplicationResource("ApplicationDefaultLanguage", "en-US");
                return new List<CultureInfo> { new CultureInfo(defaultLanguageCode) };
            }
        }

        /// <summary>Gets the current application language (thread culture).</summary>
        public CultureInfo CurrentCulture
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }

            private set
            {
                Thread.CurrentThread.CurrentUICulture = value;
            }
        }

        #endregion

        #region public functions

        /// <summary>Check whether the specified <paramref name="cultureInfo"/> is a supported UI-language.</summary>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>true if the specified <paramref name="cultureInfo"/> is a supported UI-language; otherwise false.</returns>
        public bool IsSupportedLanguage(CultureInfo cultureInfo)
        {
            return this.Languages.Contains(cultureInfo);
        }

        /// <summary>Change the user-interface culture to the culture with the specified <paramref name="cultureCode"/>.</summary>
        /// <param name="cultureCode">The culture code.</param>
        /// <returns>True if the ui language has been changed to the specified culture.</returns>
        public bool SetUserInterfaceLanguage(string cultureCode)
        {
            if (cultureCode != null)
            {
                try
                {
                    CultureInfo culture = CultureInfo.CreateSpecificCulture(cultureCode);
                    return this.SetUserInterfaceLanguage(culture);
                }
                catch (CultureNotFoundException)
                {
                }
            }

            return false;
        }

        /// <summary>Change the user-interface culture to the culture with the specified <paramref name="culture"/>.</summary>
        /// <param name="culture">The culture.</param>
        /// <returns>True if the ui language has been changed to the specified <see cref="culture"/>.</returns>
        public bool SetUserInterfaceLanguage(CultureInfo culture)
        {
            if (culture != null && this.IsSupportedLanguage(culture))
            {
                LoadTranslationDictionary(culture);
                this.CurrentCulture = culture;

                return true;
            }

            return false;
        }

        #endregion

        #region private static methods

        /// <summary>Load the translation dictionary with the specified <paramref name="culture"/>.</summary>
        /// <param name="culture">The culture info.</param>
        private static void LoadTranslationDictionary(CultureInfo culture)
        {
            string dictionaryPath = string.Format("/Assets/Localization/{0}/{1}", culture, Constants.TranslationDictionaryName);

            /* the requested dictionary is already loaded */
            if (Application.Current.Resources.MergedDictionaries.Where(d => d.Source.ToString().Contains(dictionaryPath)).Count() > 0)
            {
                return;
            }

            /* remove the existing translation dictionary */
            var translationDictionaries = Application.Current.Resources.MergedDictionaries.Where(d => d.Source.ToString().Contains(Constants.TranslationDictionaryName));
            if (translationDictionaries.Count() > 0)
            {
                Application.Current.Resources.MergedDictionaries.Remove(translationDictionaries.First());
            }

            /* add new translation dictionary for the supplied culture */
            ResourceDictionary dictionary = new ResourceDictionary
            {
                Source = new Uri(string.Format("pack://application:,,,{0}", dictionaryPath), UriKind.RelativeOrAbsolute)
            };
            Application.Current.Resources.MergedDictionaries.Add(dictionary);
        }

        #endregion
    }
}
