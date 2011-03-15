namespace HostfileManager.DataAccess
{
    using System;
    using System.IO;

    /// <summary>
    /// The <see cref="ApplicationSettingsDataAccess"/> provides
    /// file-level access to the settings of the current application instance.
    /// </summary>
    public class ApplicationSettingsDataAccess
    {
        #region private fields

        /// <summary>The settings file path.</summary>
        private readonly string settingsFilePath;

        #endregion

        #region constructor(s)

        /// <summary>Initializes a new instance of the <see cref="ApplicationSettingsDataAccess"/> class.</summary>
        /// <param name="settingsFilePath">The settings file path.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="settingsFilePath"/> cannot be null or empty.</exception>
        public ApplicationSettingsDataAccess(string settingsFilePath)
        {
            if (string.IsNullOrEmpty(settingsFilePath))
            {
                throw new ArgumentNullException("settingsFilePath");
            }

            FileInfo fileInfo = new FileInfo(settingsFilePath);
            this.settingsFilePath = fileInfo.FullName;
        }

        #endregion

        #region public functions

        /// <summary>Gets the contents of the current user's applications-settings (if it exists)</summary>
        /// <returns>The contents of the current user's application-settings if it exists; otherwise null</returns>
        public string GetApplicationSettingsXml()
        {
            return File.Exists(this.settingsFilePath) ? File.ReadAllText(this.settingsFilePath) : null;
        }

        /// <summary>Save the supplied application settings XML to the current application's settings file.</summary>
        /// <param name="xml">The xml representation of the application settings.</param>
        /// <returns>true if the supplied <paramref name="xml"/> has successfully been saved to the current application's settings file; otherwise false.</returns>
        public bool SaveApplicationSettings(string xml)
        {
            if (string.IsNullOrEmpty(xml) == false)
            {
                File.WriteAllText(this.settingsFilePath, xml);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reset the current user's application settings by deleting the settings file.
        /// </summary>
        /// <returns>True if the settings file has been removed; otherwise false.</returns>
        public bool ResetApplicationSettings()
        {
            if (File.Exists(this.settingsFilePath))
            {
                File.Delete(this.settingsFilePath);
                return true;
            }

            return false;
        }

        #endregion
    }
}
