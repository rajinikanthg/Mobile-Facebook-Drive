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
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Phone.Controls;
using MobileFacebookDrive.UserInterface;


namespace MobileFacebookDrive.Facebook
{
    /// <summary>
    /// Model class for Facebook, which does authentication, gets & puts data
    /// </summary>
    public class FacebookManager
    {
        /// <summary>
        /// MobileFacebookDrive application application ID and secret
        /// <summary>
        private String clientId = "509173369095969";
        private String clientSecret = "b53804d4b736ba6f67045c1da9785751";

        /// <summary>
        /// storing authentication token locally
        /// <summary>
        private String _accessToken;
        private WebBrowser _webBrowser;

        /// <summary>
        /// callback 
        /// <summary>
        private Action loginReturnFunction;

        /// <summary>
        /// OAuth client authentication to get the signature and access token
        /// <summary>
        FacebookOAuthClient FBclient;

        /// <summary>
        /// Constructor
        /// </summary>
        public FacebookManager()
        {
            /// <summary>
            /// initialize the authentication process
            /// <summary>
            
        }

        /// <summary>
        /// getting access token for public information which might not be used for this applciation
        /// <summary>
        public void logIn(Action function)
        {

            String url = "https://graph.facebook.com/oauth/access_token?client_id=" + clientId + "&client_secret=" + clientSecret + "&grant_type=client_credentials";

            loginReturnFunction = function;

            WebClient client = new WebClient();

            // async request for getting access token
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(onNavigated);
            client.OpenReadAsync(new Uri(url));
        }

        /// <summary>
        /// handler getting invoked when facebook reply with response
        /// <summary>
        void onNavigated(object sender, OpenReadCompletedEventArgs args)
        {
            String resultString;

            if (args.Error == null)
            {
                using (StreamReader reader = new StreamReader(args.Result))
                {
                    resultString = reader.ReadToEnd();
                    if (resultString.StartsWith("access_token="))
                    {
                        // access token storing locally
                        _accessToken = resultString.Replace("access_token=", "");
                    }
                }
            }
            // callback
            callLoginReturnFunction();
        }

        /// <summary>
        /// callback function
        /// <summary>

        void callLoginReturnFunction()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => { loginReturnFunction(); });
        }

        /// <summary>
        /// getting info from facebook depends on query string
        /// <summary>
        public void getFromFB(String url, Action<IDictionary<String, object>> retf)
        {
            FacebookClient _fbClient = new FacebookClient(_accessToken);
            _fbClient.GetCompleted += _fbClient_GetCompleted;
            // async request
            _fbClient.GetAsync(url, null, retf);
        }

        /// <summary>
        /// will be called when the data is ready from facebook
        /// <summary>
        void _fbClient_GetCompleted(object sender, FacebookApiEventArgs e)
        {
            IDictionary<string, object> result = (IDictionary<string, object>)e.GetResultData();

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                (e.UserState as Action<IDictionary<string, object>>)(result);
            });
        }

        /// <summary>
        /// picture post to facebook
        /// <summary>
        public void postToFB(String url, byte[] data, Action retf)
        {
            FacebookClient _fbClient = new FacebookClient(_accessToken);
            var parameters = new Dictionary<string, object>();
            parameters["message"] = "WP7 app presentation";            

            parameters["file"] = new FacebookMediaObject
            {
                ContentType = "image/jpeg",
                FileName = "image.jpg"
            }.SetValue(data);

            _fbClient.PostCompleted += _fbClient_PostCompleted;
             // async request
            _fbClient.PostAsync(url, parameters, retf);
        }

        /// <summary>
        /// called when the pictured is posted
        /// <summary>
        void _fbClient_PostCompleted(object sender, FacebookApiEventArgs e)
        {
            // when the picture is not posted successfully it returns here
            if (e.Cancelled || e.Error != null)
            {
                return;
            }

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                (e.UserState as Action)();
            });
        }

        /// <summary>
        ///  gets authentication token for extended permissions    
        /// <summary>
        public void loginUserToFB(Panel container, Action function)
        {
            loginReturnFunction = function;
            _webBrowser = new WebBrowser();
            _webBrowser.IsScriptEnabled = true;

            // there are the permissions I am requesting at present
            // based on the permissions facebook gives the token
            // facebook provides different levels of permissions depends on what data you are fetching.
            // for extended permission you need to request explicitly.
            string[] extendedPermissions = new[] {
                "user_photos", "read_stream", "friends_photos", "publish_stream","user_events", "create_event", "rsvp_event", "offline_access", "photo_upload"
            };

            FBclient = new FacebookOAuthClient { AppId = clientId, AppSecret = clientSecret };
            //new FacebookClient(appId, appSecret);

            var parameters = new Dictionary<string, object>()
            {
                { "client_id", clientId }, // this is needed now..
                { "response_type","token" },
                { "display", "touch" },
                { "redirect_uri", "https://www.facebook.com/connect/login_success.html"}
            };

            if (extendedPermissions != null && extendedPermissions.Length > 0)
            {
                var scope = new StringBuilder();
                scope.Append(string.Join(",", extendedPermissions));
                parameters["scope"] = scope.ToString();
            }

            var loginUrl = FBclient.GetLoginUrl(parameters);

             container.Children.Add(_webBrowser);
            _webBrowser.Navigated += userLogin_Navigated;
            _webBrowser.Navigate(loginUrl);
        }

        /// <summary>
        /// called when facebook written extended permissions request
        /// this contain the authentication token for fetching the photos and albums
        /// <summary>
        
        void userLogin_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            FacebookOAuthResult result;
            if (FBclient.TryParseResult(e.Uri, out result))
            {
                if (result.IsSuccess)
                {
                    _accessToken = result.AccessToken;
                }
                _webBrowser.Visibility = System.Windows.Visibility.Collapsed;
                callLoginReturnFunction();
            }

        }

        /// <summary>
        // handles signout event
        /// <summary>
        public void signOut()
        {
            /// <summary>
            /// set the access token null when the user taps signout option.
            /// <summary>
            _accessToken = null;
        }
    }
}
