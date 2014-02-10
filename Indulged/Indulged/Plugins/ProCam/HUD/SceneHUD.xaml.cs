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

                Label.Text = sceneStrings[_currentIndex];
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

                sceneStrings.Clear();
                foreach (CameraSceneMode mode in _supportedSceneModes)
                {
                    switch (mode)
                    { 
                        case CameraSceneMode.Auto:
                            sceneStrings.Add("Auto");
                            break;
                        case CameraSceneMode.Backlit:
                            sceneStrings.Add("Backlit");
                            break;
                        case CameraSceneMode.Beach:
                            sceneStrings.Add("Beach");
                            break;
                        case CameraSceneMode.Candlelight:
                            sceneStrings.Add("Candle light");
                            break;
                        case CameraSceneMode.Landscape:
                            sceneStrings.Add("Landscape");
                            break;
                        case CameraSceneMode.Macro:
                            sceneStrings.Add("Macro");
                            break;
                        case CameraSceneMode.Night:
                            sceneStrings.Add("Night");
                            break;
                        case CameraSceneMode.NightPortrait:
                            sceneStrings.Add("Night Portrait");
                            break;
                        case CameraSceneMode.Portrait:
                            sceneStrings.Add("Portrait");
                            break;
                        case CameraSceneMode.Snow:
                            sceneStrings.Add("Snow");
                            break;
                        case CameraSceneMode.Sport:
                            sceneStrings.Add("Sport");
                            break;
                        case CameraSceneMode.Sunset:
                            sceneStrings.Add("Sunset");
                            break;
                    }
                }

  
                RebuildSceneOptions();
            }
        }

        private List<string> sceneStrings = new List<string>();

        // Constructor
        public SceneHUD()
        {
            InitializeComponent();
        }

        private void RebuildSceneOptions()
        {
            RadioGroupPanel.Children.Clear();

            foreach(var scene in sceneStrings)
            {
                RadioButton button = new RadioButton();
                button.GroupName = "sceneMode";
                button.Content = scene;
                button.Style = (Style)App.Current.Resources["HUDRadioButtonStyle"];
                button.Margin = new Thickness(0, 4, 4, 0);
                button.VerticalAlignment = VerticalAlignment.Center;
                button.Checked += (sender, e) => 
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
