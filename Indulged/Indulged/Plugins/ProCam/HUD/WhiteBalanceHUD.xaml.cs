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
    public partial class WhiteBalanceHUD : UserControl
    {
        private int _currentWhiteBalanceIndex;
        public int CurrentWhiteBalanceIndex
        {
            get
            {
                return _currentWhiteBalanceIndex;
            }

            set
            {
                _currentWhiteBalanceIndex = value;

                Label.Text = whiteBalanceStrings[_currentWhiteBalanceIndex];
                RadioButton button = RadioGroupPanel.Children[_currentWhiteBalanceIndex] as RadioButton;

                if (button.IsChecked == false)
                {
                    button.IsChecked = true;
                }
            }
        }

        private List<object> _supportedWhiteBalances;
        public List<Object> SupportedWhiteBalances
        {
            get
            {
                return _supportedWhiteBalances;
            }

            set
            {
                _supportedWhiteBalances = value;

                whiteBalanceStrings.Clear();
                for(int i = 0; i < _supportedWhiteBalances.Count; i++)
                {
                    if(i == 0)
                    {
                        whiteBalanceStrings.Add("Auto");
                    }
                    else
                    {
                        WhiteBalancePreset wb = (WhiteBalancePreset)_supportedWhiteBalances[i];
                        switch (wb)
                        {
                            case WhiteBalancePreset.Candlelight:
                                whiteBalanceStrings.Add("Candle light");
                                break;
                            case WhiteBalancePreset.Cloudy:
                                whiteBalanceStrings.Add("Cloudy");
                                break;
                            case WhiteBalancePreset.Daylight:
                                whiteBalanceStrings.Add("Day light");
                                break;
                            case WhiteBalancePreset.Flash:
                                whiteBalanceStrings.Add("Flash");
                                break;
                            case WhiteBalancePreset.Fluorescent:
                                whiteBalanceStrings.Add("Fluorescent");
                                break;
                            case WhiteBalancePreset.Tungsten:
                                whiteBalanceStrings.Add("Tungsten");
                                break;
                        }
                    }
                }

                RebuildWhiteBalanceOptions();
            }
        }

        private List<string> whiteBalanceStrings = new List<string>();

        // Constructor
        public WhiteBalanceHUD()
        {
            InitializeComponent();
        }

        private void RebuildWhiteBalanceOptions()
        {
            RadioGroupPanel.Children.Clear();

            foreach(var wb in whiteBalanceStrings)
            {
                RadioButton button = new RadioButton();
                button.GroupName = "whiteBalance";
                button.Content = wb;
                button.Style = (Style)App.Current.Resources["HUDRadioButtonStyle"];
                button.Margin = new Thickness(0, 4, 4, 0);
                button.VerticalAlignment = VerticalAlignment.Center;
                button.Checked += (sender, e) => 
                {
                    int index = RadioGroupPanel.Children.IndexOf((RadioButton)sender);
                    CurrentWhiteBalanceIndex = index;
                };
                RadioGroupPanel.Children.Add(button);
            }
        }
    }
}
