using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace GUI.Converters
{
    public class BooleanToVisibilityConverterTwoWay: BooleanConverter<Visibility>
    {
        public BooleanToVisibilityConverterTwoWay() :
            base(Visibility.Visible, Visibility.Collapsed)
        { }
    }
}
