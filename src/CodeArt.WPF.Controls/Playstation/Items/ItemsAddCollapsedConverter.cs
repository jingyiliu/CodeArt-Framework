﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Globalization;
using System.Collections;

using CodeArt.WPF.UI;
using CodeArt.Util;


namespace CodeArt.WPF.Controls.Playstation
{
    internal class ItemsAddCollapsedConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var count = (int)values[0];
            var maxLines = (int)values[1];
            return count >= maxLines ? Visibility.Collapsed : Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public readonly static ItemsAddCollapsedConverter Instance = new ItemsAddCollapsedConverter();
    }
}