using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Indulged.Plugins.ProFX.Filters
{
    public class FilterButton : Button
    {
        // Reference to filter
        public FilterBase Filter {get; set;}

        // Template elements
        private Ellipse selectedIndicator;
        private ContentControl contentContainer;

        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("Selected", typeof(bool), typeof(FilterButton), new PropertyMetadata(OnSelectedPropertyChanged));

        public bool Selected
        {
            get
            {
                return (bool)GetValue(SelectedProperty);
            }
            set
            {
                SetValue(SelectedProperty, value);
            }
        }

        public static void OnSelectedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((FilterButton)sender).OnSelectedChanged();
        }

        protected virtual void OnSelectedChanged()
        {
            if (Selected)
            {
                selectedIndicator.Opacity = 1;
                contentContainer.Foreground = new SolidColorBrush(Color.FromArgb(0xff, 0x6d, 0x87, 0xf6));
            }
            else
            {
                selectedIndicator.Opacity = 0;
                contentContainer.Foreground = new SolidColorBrush(Color.FromArgb(0xff, 0x77, 0x81, 0xaa));
            }
            
            
        }

        // Constructor
        public FilterButton()
            : base()
        {
            DefaultStyleKey = typeof(FilterButton);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            selectedIndicator = GetTemplateChild("SelectionIndicator") as Ellipse;
            selectedIndicator.Opacity = Selected ? 1 : 0;

            contentContainer = GetTemplateChild("ContentContainer") as ContentControl;
        }
    }
}
