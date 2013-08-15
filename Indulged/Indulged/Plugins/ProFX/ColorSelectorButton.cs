using Indulged.API.Avarice.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Coding4Fun.Toolkit.Controls;
using Indulged.API.Avarice.Events;

namespace Indulged.Plugins.ProFX
{
    [TemplatePart(Name="ButtonBackground", Type=typeof(Border))]
    public class ColorSelectorButton : System.Windows.Controls.Button
    {
        // Events
        public event EventHandler SelectedColorChanged;

        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorSelectorButton), new PropertyMetadata(OnSelectedColorPropertyChanged));

        public Color SelectedColor
        {
            get
            {
                return (Color)GetValue(SelectedColorProperty);
            }
            set
            {
                SetValue(SelectedColorProperty, value);
            }
        }

        public static void OnSelectedColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ColorSelectorButton)sender).OnSelectedColorChanged();
        }

        protected virtual void OnSelectedColorChanged()
        {
            if (backgroundButton == null)
                return;

            backgroundButton.Background = new SolidColorBrush(SelectedColor);
        }

        private Border backgroundButton;


        // Constructor
        public ColorSelectorButton()
            : base()
        {
            DefaultStyleKey = typeof(ColorSelectorButton);

            // Default values
            SelectedColor = Colors.Black;

            // Events
            Click += OnColorSelectorButtonClick;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            backgroundButton = GetTemplateChild("ButtonBackground") as Border;
            backgroundButton.Background = new SolidColorBrush(SelectedColor);
        }

        protected ColorHexagonPicker colorPicker;

        protected void OnColorSelectorButtonClick(object sender, RoutedEventArgs e)
        {
            // Create the color picker view
            colorPicker = new ColorHexagonPicker();
            colorPicker.Width = 500;
            colorPicker.Height = 450;
            colorPicker.Margin = new Thickness(0, 25, 0, 0);
            colorPicker.Color = SelectedColor;

            ModalPopup dialog = ModalPopup.Show(colorPicker, "Choose Color", new List<string> { "OK", "Cancel" });
            dialog.DismissWithButtonClick += (s, args) =>
            {
                int buttonIndex = (args as ModalPopupEventArgs).ButtonIndex;
                if (buttonIndex == 0)
                {
                    if (colorPicker.Color != SelectedColor)
                    {
                        SelectedColor = colorPicker.Color;

                        if (SelectedColorChanged != null)
                            SelectedColorChanged(this, null);
                    }

                }
            };

        }
            

    }
}
