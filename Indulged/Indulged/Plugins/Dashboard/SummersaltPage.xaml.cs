using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Indulged.Plugins.Dashboard
{
    public partial class SummersaltPage : UserControl, IDashboardPage
    {
        public string BackgroundImageUrl
        {
            get
            {
                return "/Assets/Chrome/PreludeBackground.png";
            }
        }

        // Constructor
        public SummersaltPage()
        {
            InitializeComponent();
        }
    }
}
