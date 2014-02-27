using Indulged.API.Anaconda;
using Indulged.API.Anaconda.Events;
using Indulged.API.Avarice.Controls.SupportClasses;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using Indulged.API.Cinderella.Models;
using Indulged.Plugins.Group;
using Indulged.Plugins.Group.events;
using Indulged.Resources;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Indulged.Plugins.PhotoCollection
{
    public partial class DeletePhotoSetView : UserControl, IModalPopupContent
    {
        // Constructor
        public DeletePhotoSetView()
        {
            InitializeComponent();
        }

        public void ShowCompleteMessage(string text)
        {
            StatusProgressBar.Visibility = Visibility.Collapsed;
            StatusLabel.Text = text;
        }

        public void OnPopupRemoved()
        {
            
        }
    }
}
