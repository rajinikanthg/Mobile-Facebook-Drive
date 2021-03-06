﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace MobileFacebookDrive
{
    /// <summary>
    /// Splash screen.
    /// </summary>
    public partial class Splash : PhoneApplicationPage
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Splash()
        {
            InitializeComponent();
            this.performanceProgressBar.IsIndeterminate = true;           
        }
    }
}