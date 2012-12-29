using System;
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
using System.Windows.Media.Imaging;

namespace MobileFacebookDrive.UserInterface
{
    public partial class ImagePage : PhoneApplicationPage
    {
        
        private BitmapImage bitmapImage;

        // GestureListener from ToolKit
        private GestureListener gesture;

        public ImagePage()
        {
            InitializeComponent();
            gesture = GestureService.GetGestureListener(ContentPanel);
            gesture.DragCompleted += new EventHandler<DragCompletedGestureEventArgs>(gesture_DragCompleted);
        }

        void gesture_DragCompleted(object sender, DragCompletedGestureEventArgs e)
        {
            // Left or Right
            if (e.HorizontalChange > 0)
            {
                // index into previous image and loop over when it reaches end
                App.selectedImageIndex--;
                if (App.selectedImageIndex < 0) App.selectedImageIndex = App.allImages.Count - 1;
            }
            else
            {
                //index into next image and loop over when it reaches end
                App.selectedImageIndex++;
                if (App.selectedImageIndex > (App.allImages.Count - 1)) App.selectedImageIndex = 0;
            }
         
            LoadImage();
        }

        // Load Image from url
        private void LoadImage()
        {
            bitmapImage = new BitmapImage(new Uri(App.allImages[App.selectedImageIndex].source_url, UriKind.RelativeOrAbsolute));
            image.Source = bitmapImage;
            // for deep zoom feature
             //new DeepZoomImageTileSource(new System.Uri(App.allImages[App.selectedImageIndex].source_url, UriKind.RelativeOrAbsolute));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Find selected image index from parameters
            IDictionary<string, string> parameters = this.NavigationContext.QueryString;
            if (parameters.ContainsKey("SelectedIndex"))
            {
                App.selectedImageIndex = Int32.Parse(parameters["SelectedIndex"]);
            }
            else
            {
                App.selectedImageIndex = 0;
            }
           
            LoadImage();
        }

       
    }
}