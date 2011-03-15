namespace HostfileManager.Model.Base
{
    /// <summary>
    /// The <see cref="HostfileEntryType"/> enumeration contains
    /// the different types of <see cref="IHostfileEntry"/> implementations.
    /// </summary>
    public enum HostfileEntryType
    {
        /// <summary>No type defined (default).</summary>
        NotSet,

        /// <summary>Hostfile entry type "Domain" (e.g. "example.com").</summary>
        Domain,

        /// <summary>Hostfile entry type "Host" (e.g. "127.0.0.1").</summary>
        Host,

        /// <summary>Hostfile entry type "HostGroup". A "HostGroup" is a collection of "Hosts".</summary>
        HostGroup,

        /// <summary>Hostfile entry type "Comment" (e.g. "# Example Shop Development entries").</summary>
        Comment
    }
}
