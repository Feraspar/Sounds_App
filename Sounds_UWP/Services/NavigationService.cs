using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Sounds_UWP.Services
{
    /// <summary>
    /// Сервис для навигации далее по странице и назад
    /// </summary>
    public class NavigationService
    {
        private Frame GetFrame()
        {
            return (Frame)Windows.UI.Xaml.Window.Current.Content;
        }
        public void Navigate(Type sourcePage, object parameter = null)
        {
            var frame = GetFrame();
            frame.Navigate(sourcePage, parameter);
        }

        public void NavigateToBack()
        {
            var frame = GetFrame();
            if (frame.CanGoBack)
                frame.GoBack();
        }
    }
}
