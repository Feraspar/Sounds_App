using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Sounds_UWP.Converters
{
    /// <summary>
    /// Конвертер для смены изображения в зависимости от состояния
    /// </summary>
    public class ButtonImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isPlaying)
            {
                var imagePath = isPlaying ? "Assets/pause.png" : "Assets/play.png";
                return new BitmapImage(new Uri($"ms-appx:///{imagePath}"));
            }

            return new BitmapImage(new Uri("ms-appx:///Assets/play.png"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
