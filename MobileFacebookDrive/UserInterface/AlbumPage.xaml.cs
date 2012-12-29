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
using Microsoft.Phone.Shell;

namespace MobileFacebookDrive.UserInterface
{
    public partial class AlbumPage : PhoneApplicationPage
    {
        // constructor
        public AlbumPage()
        {
            InitializeComponent();
            // gets the album header and set as page title.
            var k =  PhoneApplicationService.Current.State["AlbumHeader"];
            this.PageTitle.Text = (string)k;
            // add album images to AlbumImageListBox
            AlbumImagesListBox.ItemsSource = App.albumImages;            
        }


        // moves to album image page when you select from the album collection.
        private void AlbumImagesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AlbumImagesListBox.SelectedIndex == -1) return;
            this.NavigationService.Navigate(new Uri("/UserInterface/AlbumImagePage.xaml?SelectedIndex=" + AlbumImagesListBox.SelectedIndex, UriKind.Relative));
        }

        // handles all the navigation part with in the album page
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // back button
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back)
            {
                AlbumImagesListBox.ItemsSource = App.albumImages;
                AlbumImagesListBox.SelectedIndex = -1;
                return;
            }     
        }

        // on back key press clears the album images
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            App.albumImages.Clear();
        }
        
    }
}