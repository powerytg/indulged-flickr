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
    public partial class NavigatorSection : UserControl
    {
        // Title
        public string Title {
            get
            {
                return TitleLabel.Text;
            }

            set
            {
                TitleLabel.Text = value;
            }
        }

        public NavigatorSection()
        {
            InitializeComponent();
        }
    }
}
