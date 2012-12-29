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
using Facebook;
using System.ComponentModel;

namespace MobileFacebookDrive.Models
{
    public class FacebookItemViewModel : INotifyPropertyChanged
    {
        JsonObject _post;

        /// <summary>
        /// Constructor
        /// </summary>
        public FacebookItemViewModel(JsonObject post)
        {
            _post = post;                
        }

        /// <summary>
        /// Property for category source url
        /// </summary>
         public String source_url
        {
            get
            {
                return _post["source"] as String;
            }
        }

         /// <summary>
         /// Property for category id
         /// </summary>
         public String id
         {
             get
             {
                 return _post["id"] as String;
             }
         }

         /// <summary>
         /// Property for category name
         /// </summary>
         public String name
         {
             get
             {
                 return _post["name"] as String;
             }
         }


         public event PropertyChangedEventHandler PropertyChanged;

         private void NotifyPropertyChanged(String propertyName)
         {
             PropertyChangedEventHandler handler = PropertyChanged;
             if (null != handler)
             {
                 handler(this, new PropertyChangedEventArgs(propertyName));
             }
         }

    }
}
