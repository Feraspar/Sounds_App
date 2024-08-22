using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Sounds_UWP.Converters
{
    public class ScaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double actualSize && parameter is ScaleParameters scaleParameters)
            {
                double scale = actualSize / 800.0; // Базовая ширина, например, 800
                return Math.Max(scaleParameters.MinScale, Math.Min(scaleParameters.MaxScale, scale));
            }
            return 1.0; // Возвращаем масштаб по умолчанию
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
