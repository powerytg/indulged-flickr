using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Indulged.Plugins.ProCamera
{
    public class CameraSettingsButton : Button
    {
        public static readonly DependencyProperty DisplayValueProperty = DependencyProperty.Register("DisplayValue", typeof(string), typeof(CameraSettingsButton), new PropertyMetadata(OnDisplayValuePropertyChanged));

        public string DisplayValue
        {
            get
            {
                return (string)GetValue(DisplayValueProperty);
            }
            set
            {
                SetValue(DisplayValueProperty, value);
            }
        }

        public static void OnDisplayValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((CameraSettingsButton)sender).OnDisplayValueChanged();
        }

        protected virtual void OnDisplayValueChanged()
        {
        }

        public static readonly DependencyProperty DisplayIconProperty = DependencyProperty.Register("DisplayIcon", typeof(BitmapImage), typeof(CameraSettingsButton), new PropertyMetadata(OnDisplayIconPropertyChanged));

        public BitmapImage DisplayIcon
        {
            get
            {
                return (BitmapImage)GetValue(DisplayIconProperty);
            }
            set
            {
                SetValue(DisplayIconProperty, value);
            }
        }

        public static void OnDisplayIconPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((CameraSettingsButton)sender).OnDisplayIconChanged();
        }

        protected virtual void OnDisplayIconChanged()
        {
        }
    }
}
