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
    public partial class MainPage : PhoneApplicationPage
    {

        private Popup popup;
        private BackgroundWorker backroungWorker;

       // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.performanceProgressBar.IsIndeterminate = true;
            ShowPopup();
            App.facebookManager.loginUserToFB(ContentPanel, Login_Response);                               
        }

        private void Login_Response()
        {
            NavigationService.Navigate(new Uri("/UserInterface/FacebookPhotosView.xaml?", UriKind.Relative));
        }

        private void ShowPopup()
        {
            this.popup = new Popup();
            this.popup.Child = new Splash();
            this.popup.IsOpen = true;
            StartLoadingData();
        }

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