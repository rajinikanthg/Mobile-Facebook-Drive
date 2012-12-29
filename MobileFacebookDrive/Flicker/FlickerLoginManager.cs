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
using System.IO;
using FlickrNet;
using Microsoft.Phone.Controls;
using System.Collections.Generic;

namespace MobileFacebookDrive.Flicker
{
    /// <summary>
    /// Manages flicker authentication process
    /// <summary>
    public class FlickerLoginManager
    {
        /// <summary>
        /// flickr api key
        /// <summary>
        private String flickrKey = "7c744c691c6e1d1cc95a4244cee1a1e5";
        /// <summary>
        /// flickr secret
        /// <summary>
        private String flickrSecret = "7d6df128f588e132";

        /// <summary>
        /// access token
        /// <summary>
        private String _flickrAccessToken;

        /// <summary>
        /// frob
        /// <summary>
        private String frob;

        /// <summary>
        /// browser session
        /// <summary>
        private WebBrowser _webBrowser;

        /// <summary>
        /// call back
        /// <summary>
        private Action loginReturnFunction;
        

        Panel container;


        Flickr flickr;

        /// <summary>
        /// constructor
        /// <summary>
        public FlickerLoginManager()
        {
            flickr = new Flickr(flickrKey, flickrSecret);
        }

        /// <summary>
        /// gets the frob
        /// <summary>

        public void getFlickerFrob()
        {
            flickr.AuthGetFrobAsync(onReplyFrob);
        }

        /// <summary>
        /// Handles on Frob reply
        /// <summary>

        void onReplyFrob(FlickrResult<String> result)
        {
            if (result.HasError)
            {
                MessageBox.Show(result.Error.Message);
            }
            else
            {
                frob = result.Result;
                MessageBox.Show(frob);

                string url = flickr.AuthCalcUrl(frob, FlickrNet.AuthLevel.Write);
                Uri uri = new Uri(url);
                container.Children.Add(_webBrowser);
                _webBrowser.Navigate(uri);           

            }
        }

        /// <summary>
        /// Authentication
        /// <summary>

        public void authenticateFlicker(Panel cont, Action function)
        {
            loginReturnFunction = function;
            container = cont;
            flickr.AuthGetFrobAsync(onReplyFrob);
        }

        /// <summary>
        /// gets access token
        /// <summary>
        public void getFlickerAccessToken()
        {
            string url = flickr.AuthCalcUrl(frob, FlickrNet.AuthLevel.Write);
            Uri uri = new Uri(url);
           _webBrowser = new WebBrowser();
            _webBrowser.IsScriptEnabled = true;

            container.Children.Add(_webBrowser);
            _webBrowser.Navigate(uri);
            _webBrowser.Navigated += flickrLogin_Navigated;         
            
        }

        /// <summary>
        /// call back
        /// <summary>

        void callLoginReturnFunction()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => { loginReturnFunction(); });
        }

        /// <summary>
        /// handles after getting accessing token
        /// <summary>
        void onReplyAccessToken(FlickrResult<Auth> auth)
        {
            if (auth.HasError != true)
            {
                _flickrAccessToken = auth.Result.Token;
                MessageBox.Show(_flickrAccessToken);
                //flickr.PeopleGetPhotosAsync(getPhotocollection);
                _webBrowser.Visibility = System.Windows.Visibility.Collapsed;
                //callLoginReturnFunction();
            }           
        }

        /// <summary>
        /// Login navigated
        /// <summary>

        void flickrLogin_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
             flickr.AuthGetTokenAsync(frob, onReplyAccessToken);
        }

        void getPhotocollection(FlickrResult<PhotoCollection> collection)
        {
            // photo collection
        }



        /*(r =>  
    {  
        Deployment.Current.Dispatcher.BeginInvoke(() =>  
        {  
            if (r.HasError)  
            {  
                MessageBox.Show(r.Error.Message);  
            }  
            else  
            {
                _webBrowser = new WebBrowser();
                _webBrowser.IsScriptEnabled = true;

                frob = r.Result;
                MessageBox.Show(frob);
                            
                /*string url = flicker.AuthCalcUrl(frob, FlickrNet.AuthLevel.Write);
                Uri uri = new Uri(url);
                container.Children.Add(_webBrowser);
                _webBrowser.Navigated += userLogin_Navigated;
                _webBrowser.Navigate(uri); */
        //           }  
        //     });  
        //})); */


        /// <summary>
        /// authentication token
        /// <summary>
        public void getFlickerAuthenticationToken()
        {
            Flickr flicker = new Flickr(flickrKey, flickrSecret);

            try
            {
                // use the temporary Frob to get the authentication
                flicker.AuthGetTokenAsync(frob, 
                    (token =>  
                {  
                    Deployment.Current.Dispatcher.BeginInvoke(() =>  
                    {  
                        if (token.HasError)  
                        {  
                            MessageBox.Show(token.Error.Message);  
                        }  
                        else  
                        {
                            MessageBox.Show("successfully authenticated");                          
                        }  
                    });  
                }));
                
            }
            catch (FlickrException ex)
            {
                MessageBox.Show("authentication unsuccessful");                          
                
            }

        }
    }
}
