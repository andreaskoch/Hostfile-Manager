namespace HostfileManager.Logic
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// The <see cref="StringToUriConverter"/> class converts an object
    /// of type <see cref="string"/> to an object of the type <see cref="Uri"/> and back.
    /// </summary>
    public class StringToUriConverter : IValueConverter
    {
        /// <summary>Converts a value from type <see cref="string"/> to type <see cref="Uri"/>.</summary>
        /// <param name="value">The <see cref="string"/> value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The parameter that will be converted.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string text = value as string;
                if (string.IsNullOrEmpty(text) == false)
                {
                    Uri uri;
                    if (Uri.TryCreate((string)value, UriKind.Absolute, out uri))
                    {
                        return uri;
                    }
                }
            }

            return null;
        }

        /// <summary>Converts a value back from type <see cref="Uri"/> to type <see cref="string"/>.</summary>
        /// <param name="value">The <see cref="Uri"/> value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? value.ToString() : null;
        }
    }
}
