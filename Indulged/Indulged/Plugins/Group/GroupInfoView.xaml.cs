using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Indulged.API.Anaconda;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Models;
using Indulged.API.Cinderella.Events;

namespace Indulged.Plugins.Group
{
    public partial class GroupInfoView : UserControl
    {
        public FlickrGroup Group { get; set; }

        // Constructor
        public GroupInfoView()
        {
            InitializeComponent();
        }

        

    }
}
