using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.ComponentModel;
using MobileFacebookDrive.Facebook;
using Facebook;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using Microsoft.Phone;
using System.Windows.Input;
using MobileFacebookDrive.Models;
using System.Collections.ObjectModel;
using Microsoft.Phone.Shell;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Media;

namespace MobileFacebookDrive.UserInterface
{
    public partial class FacebookPhotosView : PhoneApplicationPage
    {
        private int fetch_count;
        CameraCaptureTask capture;
        List<string> albumIdList;
        FacebookPhotoManager photoManager;
        HubTile album;
        FacebookItemViewModel model;
        string hubTileUrl = "/Images/Jellyfish.jpg";

        // isolated storage for settings
        private IsolatedStorageSettings settings;

     

        public FacebookPhotosView()
        {
            InitializeComponent();
            fetch_count = 0;
            // camera capture
             capture = new CameraCaptureTask();
             settings = IsolatedStorageSettings.ApplicationSettings;
             photoManager = new FacebookPhotoManager();
             capture.Completed += new EventHandler<PhotoResult>(Capture_Completed);
             //photoManager.getPhotos("me", photo_response); 
             
            this.Loaded += new RoutedEventHandler(FacebookPhotosPage_Loaded);

        }

        // when the panorama view is loaded
        // spaning thread to fetch all the photos from the facebook
        
        private void FacebookPhotosPage_Loaded(object sender, RoutedEventArgs e)
        {
            // spanning new thread for getting pictures from facebook
            if (fetch_count == 0)
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                worker.RunWorkerAsync();
                fetch_count++;
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {           
            System.Threading.Thread.Sleep(3500);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // get the albums form the facebook
            photoManager.getAlbums("me", albums_response);
        }

        // called when the data is ready form facebook
        void photo_response(IDictionary<String, object> res)
        {
            object response;
            
            // getting all photos 
            if (res!= null && res.TryGetValue("data", out response))
            {
                foreach (JsonObject _post in (JsonArray)response)
                {
                    model = new FacebookItemViewModel(_post);
                    if (model.source_url != null)
                    {
                        // add album image source to allImages collection
                        AlbumImage image = new AlbumImage();
                        image.source_url = model.source_url;
                        hubTileUrl = model.source_url;
                        App.allImages.Add(image);
                     }                    
                }

                this.AllImagesListBox.ItemsSource = App.allImages;
            }
            else {
                    MessageBoxResult m = MessageBox.Show("Facebook null response", "Response", MessageBoxButton.OK);
                 }
        }

        // called when albums are responded.
        void albums_response(IDictionary<String, object> res)
        {
             object response;
             albumIdList = new List<string>();
            // getting all photos 
             if (res != null && res.TryGetValue("data", out response))
             {

                 foreach (JsonObject _post in (JsonArray)response)
                 {
                     model = new FacebookItemViewModel(_post);
                     if (model.id != null)
                     {
                         photoManager.getPhotos(model.id, photo_response);
                         // add hubtile to panorama view when there is an album in facebook
                         album = new HubTile();
                         album.Title = model.name;
                         album.Margin = new Thickness(2, 2, 2, 2);
                         album.GroupTag = model.id;
                         album.Background = new SolidColorBrush(Colors.Green);
                         albumIdList.Add(model.id);
                         // add albums to album specific collection
                         this.AlbumsList.Items.Add(album);
                         // tapping support
                         album.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(album_Tap);
                     }
                 }
             }
             else
             {
                 MessageBoxResult m = MessageBox.Show("Facebook null album response", "Response", MessageBoxButton.OK);
             }
                      
        }

        void album_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            photoManager.getPhotos(((Microsoft.Phone.Controls.HubTile)(sender)).GroupTag, album_grp_response);
            PhoneApplicationService.Current.State["AlbumHeader"] = ((Microsoft.Phone.Controls.HubTile)(sender)).Title;
            NavigationService.Navigate(new Uri("/UserInterface/AlbumPage.xaml?", UriKind.Relative));
          //  this.groupViewItem.Header = ((Microsoft.Phone.Controls.HubTile)(sender)).Title + "Album";
            
        }


        void album_grp_response(IDictionary<String, object> res)
        {
            object response;

            // getting all photos 
            if (res.TryGetValue("data", out response))
            {
                foreach (JsonObject _post in (JsonArray)response)
                {
                    model = new FacebookItemViewModel(_post);
                    if (model.source_url != null)
                    {
                        AlbumImage image = new AlbumImage();
                        image.source_url = model.source_url;
                        App.albumImages.Add(image);
                    }
                }                
            }                  
         }

        // camera button event handler
        private void cameraButton_Click(object sender, EventArgs e)
        {
            capture.Show();
        }


        // handler when the camera capture completed
        void Capture_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK && e.ChosenPhoto != null)
            {

                Stream stream = e.ChosenPhoto;
                int len = (int)stream.Length;

                byte[] PhoteBytes = new byte[len];
                stream.Read(PhoteBytes, 0, len);
                // prompts user asking to post photo to facebook
                // user clicks ok, it is posted to faebook
                MessageBoxResult m = MessageBox.Show("Would you like to post facebook?", "Post Facebook", MessageBoxButton.OKCancel);
                if(m == MessageBoxResult.OK)
                    photoManager.postPhotos("me", PhoteBytes, picturePosted);
            }
        }

        // navigates back to panoramaview when the picture is posted to facebook
        void picturePosted()
        {
             NavigationService.GoBack();
        }

        // handler for all images list box selection changed
        private void AllImagesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllImagesListBox.SelectedIndex == -1) return;
            this.NavigationService.Navigate(new Uri("/UserInterface/ImagePage.xaml?SelectedIndex=" + AllImagesListBox.SelectedIndex, UriKind.Relative));
        }

        // handling signout event
        private void Signout_Click(object sender, EventArgs e)
        {
            // for the movement just clearing the obtained access token
            App.facebookManager.signOut();
            NavigationService.Navigate(new Uri("/MainPage.xaml?", UriKind.Relative));
        }

        private void SlideShow_Click(object sender, EventArgs e)
        {
            // implement functionality of slide show
        }

        //handler for tapping upload button
        private void uoloadTextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            capture.Show();
        }

     }
}