namespace HostfileManager.Logic
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// The <see cref="StreamReaderExtension"/> extension class
    /// adds some utility functions to the <see cref="StreamReader"/>
    /// class.
    /// </summary>
    public static class StreamReaderExtension
    {
        /// <summary>
        /// Gets an <see cref="IEnumerable{String}"/> with all lines in the current <see cref="StreamReader"/> object.
        /// </summary>
        /// <param name="stream">A <see cref="StreamReader"/> object.</param>
        /// <returns>An <see cref="IEnumerable{String}"/> containing all lines in the specified <paramref name="stream"/>.</returns>
        public static IEnumerable<string> Lines(this StreamReader stream)
        {
            while (stream.EndOfStream == false)
            {
                yield return stream.ReadLine();
            }
        }
    }
}
