using System;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace Pic4Pic.ObjectModel
{
    [DataContract]
    public class PushNotification
    {   
        [DataMember()]
        public int NotificationType { get; set; }

        [DataMember()]
        public int ActionType { get; set; }

        [DataMember()]
        public String ActionData { get; set; }

        [DataMember()]
        public String Title { get; set; }

        [DataMember()]
        public String Message { get; set; }

        [DataMember()]
        public int SmallIcon { get; set; }

        public bool send(String googleApiServerKey, String userDeviceKey)
        {
            if (String.IsNullOrWhiteSpace(this.Title))
            {
                throw new Pic4PicException("Notification's 'Title' is null or empty");
            }

            if (String.IsNullOrWhiteSpace(this.Message))
            {
                throw new Pic4PicException("Notification's 'Message' is null or empty");
            }

            if (String.IsNullOrWhiteSpace(googleApiServerKey))
            {
                throw new Pic4PicException("Please specify Google API Server Key");
            }

            if (String.IsNullOrWhiteSpace(userDeviceKey))
            {
                throw new Pic4PicException("Please specify userDeviceKey");
            }

            String jsonData = Pic4PicUtils.SerializeObjectToJSON<PushNotification>(this);
            PushNotificationWrapper wrapper = new PushNotificationWrapper();
            wrapper.Pic4PicJsonData = jsonData;
            return wrapper.send(googleApiServerKey, userDeviceKey);
        }
    }

    [DataContract]
    public class PushNotificationWrapper
    {
        [DataMember()]
        public String Pic4PicJsonData { get; set; }

        public bool send(String googleApiServerKey, String userDeviceKey)
        {
            if (String.IsNullOrWhiteSpace(this.Pic4PicJsonData))
            {
                throw new Pic4PicException("Pic4PicJsonData is null or empty");
            }

            if (String.IsNullOrWhiteSpace(googleApiServerKey))
            {
                throw new Pic4PicException("Please specify Google API Server Key");
            }

            if (String.IsNullOrWhiteSpace(userDeviceKey))
            {
                throw new Pic4PicException("Please specify userDeviceKey");
            }

            GooglePlayCloudMessage msg = new GooglePlayCloudMessage();
            msg.registration_ids = new String[] { userDeviceKey };
            msg.data = this;
            return msg.send(googleApiServerKey);
        }
    }

    [DataContract]
    public class GooglePlayCloudMessage
    {
        public static readonly String GoogleCloudMessagingServerUrl = "https://android.googleapis.com/gcm/send";

        [DataMember()]
        public String[] registration_ids { get; set; }

        [DataMember()]
        public PushNotificationWrapper data { get; set; }

        public bool send(String googleApiServerKey) 
        {
            if (this.registration_ids == null || this.registration_ids.Length == 0)
            {
                throw new Pic4PicException("Please define the receiver. 'registration_ids' is null or empty!");
            }

            if (String.IsNullOrWhiteSpace(googleApiServerKey))
            {
                throw new Pic4PicException("Please specify Google API Server Key");
            }

            String postData = Pic4PicUtils.SerializeObjectToJSON<GooglePlayCloudMessage>(this);
            return this._Send(postData, googleApiServerKey);
        }

        protected bool _Send(String postData, String googleApiServerKey)
        {
            var dataAsBytes = Encoding.UTF8.GetBytes(postData);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GooglePlayCloudMessage.GoogleCloudMessagingServerUrl);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = dataAsBytes.Length;
            request.Headers.Add("Authorization", "key=" + googleApiServerKey);

            using (var stream = request.GetRequestStream())
            {
                stream.Write(dataAsBytes, 0, dataAsBytes.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                return (response.StatusCode == HttpStatusCode.OK);
            }
        }
    }
}
