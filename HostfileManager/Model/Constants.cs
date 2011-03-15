namespace HostfileManager.Model
{
    /// <summary>
    /// A collection of constants and configuration values
    /// for the <see cref="HostfileManager"/>.
    /// </summary>
    public class Constants
    {
        /// <summary>The path to the hosts file on Windows machines</summary>
        public const string HostsFilePath = "C:\\Windows\\System32\\drivers\\etc\\hosts";

        /// <summary>The path to the applications-settings file.</summary>
        public const string SettingsFilePath = ".settings";

        /// <summary>
        /// The file extension of profiles.
        /// </summary>
        public const string ProfileFileExtension = ".hostfileprofile";

        /// <summary>
        /// The default directory path for hosts-file profiles.
        /// </summary>
        public const string ProfileDirectory = @".\";

        /// <summary>
        /// The prefix for command line arguments.
        /// </summary>
        public const string CommandLineArgumentPrefix = "/";

        /// <summary>
        /// The command line argument for resetting the application settings
        /// </summary>
        public const string CommandLineArgumentResetApplicationSettings = "reset";

        /// <summary>
        /// The command line argument for opening the profile directory
        /// </summary>
        public const string CommandLineArgumentOpenProfileDirectory = "open-profile-directory";

        /// <summary>
        /// The command line argument for loading the application without user interface.
        /// </summary>
        public const string CommandLineArgumentConsoleOnly = "console";

        /// <summary>
        /// The command line argument for disabling the WPF hardware acceleration.
        /// </summary>
        public const string CommandLineArgumentDisableHardwareAcceleration = "disablehardwareacceleration";

        /// <summary>
        /// The command line argument for restoring Window's default hosts file content.
        /// </summary>
        public const string CommandLineArgumentRestoreDefaultHostsFileContent = "restoredefault";

        /// <summary>The command line argument for enabling a spcific group.</summary>
        public const string CommandLineArgumentComponentGroupStatusEnable = "enable";

        /// <summary>The command line argument for enabling a spcific group.</summary>
        public const string CommandLineArgumentComponentGroupStatusDisable = "disable";

        /// <summary>
        /// The command line argument for enabling/disabling hosts groups.
        /// </summary>
        public const string CommandLineArgumentPatternSetGroupStatus = "(" + CommandLineArgumentComponentGroupStatusDisable + "|" + CommandLineArgumentComponentGroupStatusEnable + ")=(.+)";

        /// <summary>
        /// The command line argument for setting a different hosts-file path (e.g. "hostsfilepath=C:\Windows\System32\drivers\etc\hosts").
        /// </summary>
        public const string CommandLineArgumentHostsFilePath = "hostsfilepath";

        /// <summary>
        /// The command line argument pattern for setting a different hosts-file path (e.g. "hostsfilepath=C:\Windows\System32\drivers\etc\hosts").
        /// </summary>
        public const string CommandLineArgumentPatternHostsFilePath = CommandLineArgumentHostsFilePath + "=(.+)";

        /// <summary>
        /// The command line argument for loading a different hosts-file (e.g. "loadprofile=./profiles/profile1.hostfileprofile").
        /// </summary>
        public const string CommandLineArgumentLoadProfile = "loadprofile";

        /// <summary>
        /// The command line argument pattern for loading a different hosts-file (e.g. "loadprofile=./profiles/profile1.hostfileprofile").
        /// </summary>
        public const string CommandLineArgumentPatternLoadProfile = CommandLineArgumentLoadProfile + "=(.+)";

        /// <summary>
        /// The command line argument for setting a specific user-interface culture (e.g. "culture=de-DE").
        /// </summary>
        public const string CommandLineArgumentSetUserInterfaceCulture = "culture";

        /// <summary>
        /// The command line argument pattern for setting a specific user-interface culture (e.g. "culture=fr-FR").
        /// </summary>
        public const string CommandLineArgumentPatternSetUserInterfaceCulture = CommandLineArgumentSetUserInterfaceCulture + "=([a-z\\-]{2,})";

        /// <summary>The file name for all translation-dictionaries.</summary>
        public const string TranslationDictionaryName = "TranslationDictionary.xaml";

        /// <summary>The regular expression pattern match group name 1.</summary>
        public const string RegularExpressionParameterGroupName1 = "value1";

        /// <summary>The regular expression pattern match group name 2.</summary>
        public const string RegularExpressionParameterGroupName2 = "value2";

        /// <summary>The regular expression pattern match group name 3.</summary>
        public const string RegularExpressionParameterGroupName3 = "value3";

        /// <summary>Hostfile syntax constant: The Comment character.</summary>
        public const string HostFileSyntaxCommentCharacter = "#";

        /// <summary>Hostfile syntax constant: The prefix for exclusive toggle-mode properties.</summary>
        public const string HostFileSyntaxControlElemenExclusiveToggleMode = "ExclusiveToggleMode:";

        /// <summary>Hostfile syntax constant: The group-toggle-mode property.</summary>
        public const string HostFileSyntaxControlElementGroupToggleMode = HostFileSyntaxControlElemenExclusiveToggleMode + "Group";

        /// <summary>Hostfile syntax constant: The host-toggle-mode property.</summary>
        public const string HostFileSyntaxControlElementHostToggleMode = HostFileSyntaxControlElemenExclusiveToggleMode + "Host";

        /// <summary>Hostfile syntax constant: The start element for groups.</summary>
        public const string HostFileSyntaxControlElementHostGroupStart = "Start-HostsGroup:";

        /// <summary>Hostfile syntax constant: The end element for groups.</summary>
        public const string HostFileSyntaxControlElementHostGroupEnd = "End-HostsGroup:";
    }
}
