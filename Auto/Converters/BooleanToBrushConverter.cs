using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Auto.Converters
{
    public class BooleanToBrushConverter: IValueConverter
    {
            public Brush TrueBrush { get; set; } = new SolidColorBrush(Color.FromRgb(52, 168, 83));
            public Brush FalseBrush { get; set; } = new SolidColorBrush(Color.FromRgb(214, 69, 69));

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is bool b)
                {
                    return b ? TrueBrush : FalseBrush;
                }
                return FalseBrush;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
    }
}
