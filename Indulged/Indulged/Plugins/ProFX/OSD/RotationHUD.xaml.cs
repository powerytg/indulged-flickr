using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Indulged.Plugins.ProFX.OSD
{
    public partial class RotationHUD : UserControl
    {
        public EventHandler OnDismiss;
        public EventHandler OnDelete;
        public EventHandler ValueChanged;

        public double Degree { get; set; }

        // Constructor
        public RotationHUD()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (OnDismiss != null)
            {
                OnDismiss(this, null);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (OnDelete != null)
            {
                OnDelete(this, null);
            }
        }

        private void AmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AmountSlider == null)
                return;

            Degree = AmountSlider.Value;
            if (ValueChanged != null)
            {
                ValueChanged(this, null);
            }
        }
    }
}
