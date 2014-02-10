using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Indulged.Plugins.ProCam.HUD
{
    public partial class MainHUD : UserControl
    {
        private Windows.Foundation.Size _currentResolution;
        public Windows.Foundation.Size CurrentResolution
        {
            get
            {
                return _currentResolution;
            }

            set
            {
                _currentResolution = value;

                int index = _supportedResolutions.IndexOf(_currentResolution);
                RadioButton button = ResolutionGroupPanel.Children[index] as RadioButton;
                button.IsChecked = true;

                ResolutionLabel.Text = GetResolutionDisplayText(_currentResolution);
            }
        }

        private List<Windows.Foundation.Size> _supportedResolutions;
        public List<Windows.Foundation.Size> SupportedResolutions
        {
            get
            {
                return _supportedResolutions;
            }

            set
            {
                _supportedResolutions = value;
                RebuildResolutionOptions();
            }
        }

        // Constructor
        public MainHUD()
        {
            InitializeComponent();
        }

        private void RebuildResolutionOptions()
        {
            ResolutionGroupPanel.Children.Clear();

            for (int i = 0; i < _supportedResolutions.Count; i++)
            {
                var res = _supportedResolutions[i];

                RadioButton button = new RadioButton();
                button.GroupName = "resolution";
                button.Style = (Style)App.Current.Resources["FXCompactRadioButtonStyle"];
                button.Margin = new Thickness(0, 0, 8, 0);
                button.VerticalAlignment = VerticalAlignment.Center;
                ResolutionGroupPanel.Children.Add(button);
            }
        }

        private string GetResolutionDisplayText(Windows.Foundation.Size res)
        {
            double result = (res.Width * res.Height) / 1000000;
            int intValue = (int)Math.Ceiling(result);

            return intValue.ToString();
        }

    }
}
