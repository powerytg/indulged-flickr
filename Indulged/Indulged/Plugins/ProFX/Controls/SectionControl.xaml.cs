using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Indulged.Plugins.ProFX.Controls
{
    public partial class SectionControl : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(SectionControl), new PropertyMetadata(OnTitlePropertyChanged));

        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public static void OnTitlePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((SectionControl)sender).OnTitleChanged();
        }

        protected virtual void OnTitleChanged()
        {
            Label.Text = Title;
        }

        // Constructor
        public SectionControl()
        {
            InitializeComponent();
        }
    }
}
