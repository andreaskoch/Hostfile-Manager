namespace HostfileManager.Model
{
    using System.IO;

    /// <summary>
    /// The <see cref="HostsFileProfile"/> model class
    /// contains all properties related to host-file profiles.
    /// </summary>
    public class HostsFileProfile
    {
        #region constructor(s)

        /// <summary>Initializes a new instance of the <see cref="HostsFileProfile"/> class.</summary>
        /// <param name="profileName">The profile name.</param>
        /// <param name="profilePath">The profile path.</param>
        private HostsFileProfile(string profileName, string profilePath)
        {
            this.ProfileName = profileName;
            this.ProfilePath = profilePath;
        }

        #endregion

        #region properties

        /// <summary>Gets or sets name of this hosts-file profile instance.</summary>
        public string ProfileName { get; set; }

        /// <summary>Gets or sets the file path to this hosts-file profile instance.</summary>
        public string ProfilePath { get; set; }

        #endregion

        #region factory methods

        /// <summary>Create a new <see cref="HostsFileProfile"/> instance from the specified <paramref name="profilePath"/>.</summary>
        /// <param name="profilePath">The profile path.</param>
        /// <returns>A new <see cref="HostsFileProfile"/> instance; null if the specified <paramref name="profilePath"/> does not exist.</returns>
        public static HostsFileProfile CreateFromPath(string profilePath)
        {
            if (File.Exists(profilePath))
            {
                FileInfo fileInfo = new FileInfo(profilePath);
                return new HostsFileProfile(fileInfo.Name.Replace(string.Concat(".", fileInfo.Extension), string.Empty), fileInfo.FullName);
            }

            return null;
        }

        #endregion
    }
}
