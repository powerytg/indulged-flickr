using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Phone.Media.Capture;

namespace Indulged.Plugins.ProCam.HUD
{
    public partial class FocusAssistHUD : UserControl
    {
        // Events
        public EventHandler FocusAssistModeChanged;

        private int _currentIndex;
        public int CurrentIndex
        {
            get
            {
                return _currentIndex;
            }

            set
            {
                _currentIndex = value;

                Label.Text = ModeStrings[_currentIndex];
                RadioButton button = RadioGroupPanel.Children[_currentIndex] as RadioButton;

                if (button.IsChecked == false)
                {
                    button.IsChecked = true;
                }
            }
        }

        private List<FocusIlluminationMode> _supportedModes;
        public List<FocusIlluminationMode> SupportedModes
        {
            get
            {
                return _supportedModes;
            }

            set
            {
                _supportedModes = value;

                ModeStrings.Clear();
                foreach (FocusIlluminationMode mode in _supportedModes)
                {
                    switch (mode)
                    {
                        case FocusIlluminationMode.Auto:
                            ModeStrings.Add("Auto");
                            break;
                        case FocusIlluminationMode.On:
                            ModeStrings.Add("On");
                            break;
                        case FocusIlluminationMode.Off:
                            ModeStrings.Add("Off");
                            break;
                    }
                }

  
                RebuildModeOptions();
            }
        }

        public List<string> ModeStrings { get; set; }

        // Constructor
        public FocusAssistHUD()
        {
            InitializeComponent();
            ModeStrings = new List<string>();
        }

        private void RebuildModeOptions()
        {
            RadioGroupPanel.Children.Clear();

            foreach (var mode in ModeStrings)
            {
                RadioButton button = new RadioButton();
                button.GroupName = "focusAssistMode";
                button.Content = mode;
                button.Style = (Style)App.Current.Resources["HUDRadioButtonStyle"];
                button.Margin = new Thickness(0, 4, 4, 0);
                button.VerticalAlignment = VerticalAlignment.Center;
                button.Checked += (sender, e) => 
                {
                    int index = RadioGroupPanel.Children.IndexOf((RadioButton)sender);
                    CurrentIndex = index;

                    if (FocusAssistModeChanged != null)
                    {
                        FocusAssistModeChanged(this, null);
                    }
                };

                RadioGroupPanel.Children.Add(button);
            }
        }
    }
}
