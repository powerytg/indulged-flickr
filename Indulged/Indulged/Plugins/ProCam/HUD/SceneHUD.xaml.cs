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
    public partial class SceneHUD : UserControl
    {
        // Events
        public EventHandler SceneModeChanged;

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

                Label.Text = SceneStrings[_currentIndex];
                RadioButton button = RadioGroupPanel.Children[_currentIndex] as RadioButton;

                if (button.IsChecked == false)
                {
                    button.IsChecked = true;
                }
            }
        }

        private List<CameraSceneMode> _supportedSceneModes;
        public List<CameraSceneMode> SupportedSceneModes
        {
            get
            {
                return _supportedSceneModes;
            }

            set
            {
                _supportedSceneModes = value;

                SceneStrings.Clear();
                foreach (CameraSceneMode mode in _supportedSceneModes)
                {
                    switch (mode)
                    { 
                        case CameraSceneMode.Auto:
                            SceneStrings.Add("Auto");
                            break;
                        case CameraSceneMode.Backlit:
                            SceneStrings.Add("Backlit");
                            break;
                        case CameraSceneMode.Beach:
                            SceneStrings.Add("Beach");
                            break;
                        case CameraSceneMode.Candlelight:
                            SceneStrings.Add("Candle light");
                            break;
                        case CameraSceneMode.Landscape:
                            SceneStrings.Add("Landscape");
                            break;
                        case CameraSceneMode.Macro:
                            SceneStrings.Add("Macro");
                            break;
                        case CameraSceneMode.Night:
                            SceneStrings.Add("Night");
                            break;
                        case CameraSceneMode.NightPortrait:
                            SceneStrings.Add("Night Portrait");
                            break;
                        case CameraSceneMode.Portrait:
                            SceneStrings.Add("Portrait");
                            break;
                        case CameraSceneMode.Snow:
                            SceneStrings.Add("Snow");
                            break;
                        case CameraSceneMode.Sport:
                            SceneStrings.Add("Sport");
                            break;
                        case CameraSceneMode.Sunset:
                            SceneStrings.Add("Sunset");
                            break;
                    }
                }

  
                RebuildSceneOptions();
            }
        }

        public List<string> SceneStrings { get; set; }

        // Constructor
        public SceneHUD()
        {
            InitializeComponent();

            SceneStrings = new List<string>();
        }

        private void RebuildSceneOptions()
        {
            RadioGroupPanel.Children.Clear();

            foreach (var scene in SceneStrings)
            {
                RadioButton button = new RadioButton();
                button.GroupName = "sceneMode";
                button.Content = scene;
                button.Style = (Style)App.Current.Resources["HUDRadioButtonStyle"];
                button.Margin = new Thickness(0, 4, 4, 0);
                button.VerticalAlignment = VerticalAlignment.Center;
                button.Click += (sender, e) => 
                {
                    int index = RadioGroupPanel.Children.IndexOf((RadioButton)sender);
                    CurrentIndex = index;

                    if (SceneModeChanged != null)
                    {
                        SceneModeChanged(this, null);
                    }
                };

                RadioGroupPanel.Children.Add(button);
            }
        }
    }
}
