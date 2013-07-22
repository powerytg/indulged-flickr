using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
    }
}
