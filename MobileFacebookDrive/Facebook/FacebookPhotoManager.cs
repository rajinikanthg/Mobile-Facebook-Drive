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
using System.Collections.Generic;

using System.Collections.ObjectModel;
using Facebook;



namespace MobileFacebookDrive.Facebook
{
    /// <summary>
    /// Model class manages facebook photos
    /// <summary>
    public class FacebookPhotoManager
    {
        /// <summary>
        /// callback
        /// <summary>
        Action<IDictionary<String, object>> _callback;

        /// <summary>
        /// getting user photos from facebook
        /// <summary>
        public void getPhotos(String objId, Action<IDictionary<String, object>> callback)
        {
            String _url = objId + "/photos";
            _callback = callback;
            App.facebookManager.getFromFB(_url, photosLoaded);      
        }

        /// <summary>
        /// To get album from facebook
        /// <summary>

        public void getAlbums(String objId, Action<IDictionary<String, object>> callback)
        {
            String _url = objId + "/albums";
            _callback = callback;
            App.facebookManager.getFromFB(_url, albumsLoaded);
        }

        /// <summary>
        /// album is loaded it will be called
        /// <summary>
        void albumsLoaded(IDictionary<String, object> r)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                _callback(r);
            });
        }

        /// <summary>
        /// album photos loaded
        /// <summary>
        void photosLoaded(IDictionary<String, object> r)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                _callback(r);
            });
        }

        /// <summary>
        /// post a picture to facebook
        /// it will be stored in facebook MobileFacebookDrive folder
        /// <summary>
        public void postPhotos(String postId, byte[] dat, Action callback)
        {
            String _url = postId + "/photos";
            App.facebookManager.postToFB(_url, dat, picturePosted);
        }

        /// <summary>
        /// called when the post is successful
        /// <summary>
        public void  picturePosted()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
               // call back
            });
        }

     }  
}
