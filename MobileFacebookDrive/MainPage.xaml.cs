using System;
using System.Windows;
using Microsoft.Phone.Controls;
using MobileFacebookDrive.Flicker;
using System.Windows.Controls.Primitives;
using MobileFacebookDrive.UserInterface;
using System.ComponentModel;
using System.Threading;

namespace MobileFacebookDrive
{
    /// <summary>
    /// Main page of application
    /// </summary>

    public partial class MainPage : PhoneApplicationPage
    {

        private Popup popup;
        private BackgroundWorker backroungWorker;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            this.performanceProgressBar.IsIndeterminate = true;
            ShowPopup();

            /// <summary>
            /// Login Authentication with facebook
            /// </summary>

            App.facebookManager.loginUserToFB(ContentPanel, Login_Response);                               
        }

        /// <summary>
        /// On login respinse move to facebook photo view
        /// </summary>

        private void Login_Response()
        {
            NavigationService.Navigate(new Uri("/UserInterface/FacebookPhotosView.xaml?", UriKind.Relative));
        }


        /// <summary>
        /// Shows the popup splash screen until authentication processes completes 
        /// with facebook
        /// </summary>
        
        private void ShowPopup()
        {
            this.popup = new Popup();
            this.popup.Child = new Splash();
            this.popup.IsOpen = true;
            StartLoadingData();
        }

        /// <summary>
        /// Fetching data from facebook is done with separate thread
        /// So that it can not effect the user interface.
        /// </summary>

        private void StartLoadingData()
        {
            backroungWorker = new BackgroundWorker();
            backroungWorker.DoWork += new DoWorkEventHandler(backroungWorker_DoWork);
            backroungWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backroungWorker_RunWorkerCompleted);
            backroungWorker.RunWorkerAsync();
        }

        void backroungWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(5000);
        }

        void backroungWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                this.popup.IsOpen = false;

            }
            );
        }

    }
}