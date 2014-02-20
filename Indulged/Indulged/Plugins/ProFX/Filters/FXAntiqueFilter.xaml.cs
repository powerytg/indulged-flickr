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
using Indulged.Resources;

namespace Indulged.Plugins.ProFX.Filters
{
    public partial class FXAntiqueFilter : FilterBase
    {
        public FXAntiqueFilter()
        {
            InitializeComponent();

            DisplayName = "antique";
            StatusBarName = "Antique";
            Category = FilterCategory.Effect;
        }

        public override bool hasEditorUI
        {
            get
            {
                return false;
            }
        }

        public override void CreateFilter()
        {
            Filter = FilterFactory.CreateAntiqueFilter();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

        
    }
}
