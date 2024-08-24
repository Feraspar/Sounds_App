using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sounds_UWP.Converters
{
    public class ItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FirstItemTemplate { get; set; }
        public DataTemplate RegularItemTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var itemsControl = ItemsControl.ItemsControlFromItemContainer(container);
            var index = itemsControl.Items.IndexOf(item);

            return index == 0 ? FirstItemTemplate : RegularItemTemplate;
        }
    }
}
