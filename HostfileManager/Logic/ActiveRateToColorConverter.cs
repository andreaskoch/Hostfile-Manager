namespace HostfileManager.Logic
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// The <see cref="ActiveRateToColorConverter"/> class converts an object
    /// of type <see cref="double"/> to a <see cref="LinearGradientBrush"/> object instance.
    /// </summary>
    public class ActiveRateToColorConverter : IValueConverter
    {
        /// <summary>Converts a value from type <see cref="double"/> to type <see cref="LinearGradientBrush"/>.</summary>
        /// <param name="value">The <see cref="double"/> value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The parameter that will be converted.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && parameter != null)
            {
                double activeRate = (double)value;

                LinearGradientBrush paramBrush = parameter as LinearGradientBrush;
                if (paramBrush != null && paramBrush.GradientStops.Count >= 2)
                {
                    /* copy the parameter brush */
                    LinearGradientBrush linearGradientBrush = paramBrush.Clone();

                    /* get start color and target color */
                    GradientStop firstGradientStop = linearGradientBrush.GradientStops.First();
                    GradientStop lastGradientStop = linearGradientBrush.GradientStops.Last();

                    Color startColor = firstGradientStop.Color;
                    Color targetColor = lastGradientStop.Color;

                    /* parameters */
                        /* The decimal value which represents a 0% active-rate (lower bound). */
                        double activeRateLowerBound = firstGradientStop.Offset;

                        /* The decimal value which represents a 100% active-rate (upper bound). */
                        double activeRateUpperBound = lastGradientStop.Offset;

                        /* The decimal value which defines the number of gradient stops used for the <see cref="LinearGradientBrush"/>. */
                        double gradientStopCount = (activeRateUpperBound - activeRateLowerBound) * 10;

                        /* This decimal value represents the gradient stop distance. */
                        double gradientStopDistance = activeRateUpperBound / gradientStopCount;

                        /* This value represents the share of the start color in comparision to the target color */
                        double shareStartColor = activeRateUpperBound - activeRate;

                    /* edge cases */
                    if (shareStartColor == activeRateLowerBound)
                    {
                        startColor = targetColor;
                    }

                    /* add start color stops to the gradient depending on the share of the start color */
                    for (double step = activeRateLowerBound; step <= shareStartColor; step = step + gradientStopDistance)
                    {
                        double offset = step;
                        GradientStop initialGradientStop = linearGradientBrush.GradientStops.Where(stop => (decimal)stop.Offset == (decimal)offset).FirstOrDefault();

                        if (initialGradientStop != null)
                        {
                            /* reset color */
                            initialGradientStop.Color = startColor;
                        }
                        else
                        {
                            /* add new gradient stop */
                            linearGradientBrush.GradientStops.Add(new GradientStop(startColor, offset));
                        }
                    }

                    return linearGradientBrush;
                }
            }

            return null;
        }

        /// <summary>Converts a value back from type <see cref="LinearGradientBrush"/> to type <see cref="double"/>.</summary>
        /// <param name="value">The <see cref="LinearGradientBrush"/> value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <exception cref="NotImplementedException">This function is not implemented. The <see cref="ActiveRateToColorConverter"/> should only be used for OneWay binding.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
