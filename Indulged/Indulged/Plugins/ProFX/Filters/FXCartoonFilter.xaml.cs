using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Nokia.Graphics.Imaging;

namespace Indulged.Plugins.ProFX.Filters
{
    public partial class FXCartoonFilter : FilterBase
    {
        private bool shouldTraceEdge = true;

        public FXCartoonFilter()
        {
            InitializeComponent();

            DisplayName = "cartoon";
            StatusBarName = "Cartoon";
        }

        protected override void CreateFilter()
        {
            Filter = FilterFactory.CreateCartoonFilter(shouldTraceEdge);
        }
       
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

        private void EdgeSwitch_Checked(object sender, RoutedEventArgs e)
        {
            shouldTraceEdge = true;
            UpdatePreviewAsync();
        }

        private void EdgeSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            shouldTraceEdge = false;
            UpdatePreviewAsync();
        }

        
    }
}
