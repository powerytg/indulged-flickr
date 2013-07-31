﻿using System;
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
    public partial class FXOilFilter : FilterBase
    {
        public FXOilFilter()
        {
            InitializeComponent();

            DisplayName = "oil";
        }

        protected override void CreateFilter()
        {
            Filter = FilterFactory.CreateOilyFilter();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

        
    }
}
