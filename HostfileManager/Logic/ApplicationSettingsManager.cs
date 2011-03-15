namespace HostfileManager.Logic
{
    using System;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Xml.Serialization;

    using DataAccess;
    using Model;

    /// <summary>
    /// The <see cref="ApplicationSettingsManager"/> class
    /// provides read and write access to the global application settings.
    /// </summary>
    public class ApplicationSettingsManager
    {
        #region private fields

        /// <summary>The key for the singleton accessor.</summary>
        private const string SingletonKey = "ApplicationSettingsManager-Instance";

        /// <summary>A flag indicating whether the <see cref="appSettings"/> value has been set yet.</summary>
        private bool appSettingsHaveBeenSet;

        /// <summary>The application settings.</summary>
        private ApplicationSettings appSettings;

        /// <summary>An instance of the <see cref="ApplicationSettingsDataAccess"/> class.</summary>
        private ApplicationSettingsDataAccess dataAccess;

        #endregion

        #region constructor(s)

        /// <summary>Prevents a default instance of the <see cref="ApplicationSettingsManager"/> class from being created.</summary>
        private ApplicationSettingsManager()
        {
        }

        #endregion

        #region singleton accessor

        /// <summary>Gets the <see cref="ApplicationSettingsManager"/> class instance for the current application.</summary>
        public static ApplicationSettingsManager Current
        {
            get
            {
                if (Application.Current.Properties.Contains(SingletonKey))
                {
                    return Application.Current.Properties[SingletonKey] as ApplicationSettingsManager;
                }

                ApplicationSettingsManager newInstance = new ApplicationSettingsManager();
                Application.Current.Properties.Add(SingletonKey, newInstance);
                return newInstance;
            }
        }

        #endregion

        #region public properties

        /// <summary>Gets the user's aplication settings.</summary>
        public ApplicationSettings Settings
        {
            get
            {
                if (this.appSettingsHaveBeenSet == false)
                {
                    this.appSettings = this.GetApplicationSettings();
                    this.appSettingsHaveBeenSet = true;
                }

                return this.appSettings;
            }
        }

        #endregion

        #region protected properties

        /// <summary>Gets an instance of the <see cref="ApplicationSettingsDataAccess"/> class.</summary>
        public ApplicationSettingsDataAccess DataAccess
        {
            get
            {
                return this.dataAccess ?? (this.dataAccess = new ApplicationSettingsDataAccess(Constants.SettingsFilePath));
            }
        }

        #endregion

        #region public functions

        /// <summary>Save the current <see cref="Settings"/> to a settings-file on the user's hard disk.</summary>
        /// <returns>True if the <see cref="Settings"/> have been saved successfully; otherwise false.</returns>
        public bool SaveApplicationSettings()
        {
            string xml = ConvertToXml(this.Settings);
            return string.IsNullOrEmpty(xml) == false && this.DataAccess.SaveApplicationSettings(xml);
        }

        /// <summary>
        /// Reset the current user's application settings by deleting the settings file.
        /// </summary>
        /// <returns>True if the settings file has been removed; otherwise false.</returns>
        public bool ResetApplicationSettings()
        {
            return this.DataAccess.ResetApplicationSettings();
        }

        #endregion

        #region private static functions

        /// <summary>
        /// Convert an <see cref="ApplicationSettings"/> class to XML.
        /// </summary>
        /// <param name="applicationSettings">An instance of an <see cref="ApplicationSettings"/> class.</param>
        /// <returns>The XML of the specified <see cref="ApplicationSettings"/> class instance if it is not null</returns>
        private static string ConvertToXml(ApplicationSettings applicationSettings)
        {
            if (applicationSettings == null)
            {
                return null;
            }

            var xmlStringBuilder = new StringBuilder();
            using (var streamWriter = new StringWriter(xmlStringBuilder))
            {
                var xmlSerializer = new XmlSerializer(applicationSettings.GetType());
                xmlSerializer.Serialize(streamWriter, applicationSettings);
                return xmlStringBuilder.ToString();
            }
        }

        /// <summary>
        /// Convert the XML back to an instance of the <see cref="ApplicationSettings"/> class.
        /// </summary>
        /// <param name="xmlCode">XML code representing an <see cref="ApplicationSettings"/> object instance.</param>
        /// <returns>An initialized <see cref="ApplicationSettings"/> object instance if the XML is valid; otherwise null</returns>
        /// <exception cref="InvalidOperationException">Cannot cast the specified <paramref name="xmlCode"/> to an instance of the type <see cref="ApplicationSettings"/>.</exception>
        private static ApplicationSettings ConvertToApplicationSettings(string xmlCode)
        {
            if (string.IsNullOrEmpty(xmlCode) == false)
            {
                using (var streamReader = new StringReader(xmlCode))
                {
                    var xmlSerializer = new XmlSerializer(typeof(ApplicationSettings));
                    return (ApplicationSettings)xmlSerializer.Deserialize(streamReader);
                }
            }

            return null;
        }

        #endregion

        #region private functions

        /// <summary>Gets the contents of the current user's applications-settings (if it exists)</summary>
        /// <returns>The contents of the current user's application-settings if it exists; otherwise null</returns>
        private ApplicationSettings GetApplicationSettings()
        {
            try
            {
                string xml = this.DataAccess.GetApplicationSettingsXml();
                ApplicationSettings settings = ConvertToApplicationSettings(xml);
                if (settings != null)
                {
                    return settings;
                }
            }
            catch (InvalidOperationException)
            {
                /* cannot parse xml */
                this.ResetApplicationSettings();
            }

            return new ApplicationSettings();
        }

        #endregion
    }
}
