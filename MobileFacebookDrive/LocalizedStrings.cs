using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MobileFacebookDrive
{
    public class LocalizedStrings
    {
          // constructor
        public LocalizedStrings() 
        {
        }

        private  static MobileFacebookDrive.AppResources localizedResources = new MobileFacebookDrive.AppResources();
        private  MobileFacebookDrive.AppResources LocalizedResources { get { return localizedResources; } }
    }

    
}
