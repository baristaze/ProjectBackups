using System;
using System.IO;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.UnitTests
{
    [DataContract()]
    public enum ObjectType
    {
        [EnumMember()]
        Undefined = 0,

        [EnumMember()]
        Notification = 1,

        [EnumMember()]
        Profile = 2,
    }

    [DataContract()]
    public enum MarkingType
    {
        [EnumMember()]
        Undefined = 0,

        [EnumMember()]
        Viewed = 1,

        [EnumMember()]
        Liked = 2,
    }

    [DataContract()]
    public class MarkingRequest
    {
        [DataMember()]
        public MarkingType MarkingType { get; set; }

        [DataMember()]
        public ObjectType ObjectType { get; set; }

        [DataMember()]
        public Guid ObjectId { get; set; }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}-{1}", this.ObjectType, this.ObjectId);
        }
    }

    [DataContract()]
    public class ClientLogRequest : BaseRequest
    {
        [DataMember]
        public List<LogBag> Logs 
        { 
            get 
            { 
                return this.logs; 
            }
            set
            {
                if (value != null)
                {
                    this.logs = value;
                }
                else
                {
                    this.logs = new List<LogBag>();
                }                
            }
        }

        private List<LogBag> logs = new List<LogBag>();
    }

    [DataContract()]
    public class JsonObjectWrapper
    {
        [DataMember()]
        public string ObjectName { get; set; }

        [DataMember()]
        public string ObjectJsonData { get; set; }
    }

    [DataContract()]
    public class UserResponseTest : BaseResponse
    {   
        [DataMember()]
        public string AuthToken { get; set; }

        [DataMember()]
        public UserProfile UserProfile { get; set; }

        [DataMember()]
        public UserProfilePics ProfilePictures { get; set; }
        
        [DataMember()]
        public List<PicturePair> OtherPictures { get { return this.otherPictures; } }
        protected List<PicturePair> otherPictures = new List<PicturePair>();

        [DataMember()]
        public List<NameValue> Settings { get { return this.settings; } }
        protected List<NameValue> settings = new List<NameValue>();

        public UserResponseTest() : base () { }

        public UserResponseTest(int errorCode, string errorMessage) : base(errorCode, errorMessage) { }
    }

    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void Test_SerializationOfUserSettings()
        {
            UserResponseTest userResponse = new UserResponseTest();
            userResponse.Settings.Add(new NameValue("Split", "1"));
            userResponse.Settings.Add(new NameValue("FacebookPermissions", "email,user_hometown,user_birthday"));

            string json = this.SerializeObject<UserResponseTest>(userResponse);
            Console.WriteLine(json);

            /*
            UserResponseTest copy = this.DeserializeObject<UserResponseTest>(json);
            if (copy == null)
            {
                throw new ApplicationException("Deserialization result is null");
            }

            if (copy.Settings.Count != userResponse.Settings.Count)
            {
                throw new ApplicationException("Setting counts are not equal");
            }

            for (int x = 0; x < userResponse.Settings.Count; x++)
            {
                if (copy.Settings[x].Name != userResponse.Settings[x].Name)
                {
                    throw new ApplicationException("Setting names are not equal");
                }

                if (copy.Settings[x].Value != userResponse.Settings[x].Value)
                {
                    throw new ApplicationException("Setting values are not equal");
                }
            }
            */
        }
   
        [TestMethod]
        public void Test_SerializationOfClientLogBags()
        {
            LogBag logBag = new LogBag();
            if (logBag.Pairs == null) 
            {
                throw new ApplicationException("logBag.Pairs are null");
            }
            logBag.Pairs.Add(new LogProperty("Fruit", "Apple"));

            ClientLogRequest request = new ClientLogRequest();
            if (request.Logs == null)
            {
                throw new ApplicationException("request.Logs are null");
            }
            request.Logs.Add(logBag);

            Console.WriteLine("Here");

            string json = this.SerializeObject<ClientLogRequest>(request);
            Console.WriteLine(json);

            ClientLogRequest copy = this.DeserializeObject<ClientLogRequest>(json);
            if (copy == null)
            {
                throw new ApplicationException("Deserialization result is null");
            }

            if (copy.Logs.Count != request.Logs.Count) 
            {
                throw new ApplicationException("Log counts are not equal");
            }

            for (int x = 0; x < request.Logs.Count; x++) {
                if (copy.Logs[x].Pairs.Count != request.Logs[x].Pairs.Count)
                {
                    throw new ApplicationException("LogProperty counts are not equal");
                }
            }
        }

        [TestMethod]
        public void Test_SerializationOfMarkingRequest()
        {
            // MarkingRequest

            MarkingRequest markingRequest = new MarkingRequest();
            markingRequest.MarkingType = MarkingType.Liked;
            markingRequest.ObjectType = ObjectType.Profile;
            markingRequest.ObjectId = Guid.NewGuid();

            string json = this.SerializeObject<MarkingRequest>(markingRequest);
            Console.WriteLine(json);

            MarkingRequest copy = this.DeserializeObject<MarkingRequest>(json);
            if(copy == null)
            {
                throw new ApplicationException("Deserialization result is null");
            }
            
            if(markingRequest.MarkingType != copy.MarkingType)
            {
                throw new ApplicationException("MarkingTypes are not equal");
            }

            if(markingRequest.ObjectType != copy.ObjectType)
            {
                throw new ApplicationException("ObjectTypes are not equal");
            }

            if(markingRequest.ObjectId != copy.ObjectId)
            {
                throw new ApplicationException("ObjectIds are not equal");
            }

            JsonObjectWrapper wrapper = new JsonObjectWrapper();
            wrapper.ObjectName = "MarkingRequest";
            wrapper.ObjectJsonData = json;

            string jsonWrapper = this.SerializeObject<JsonObjectWrapper>(wrapper);
            Console.WriteLine(jsonWrapper);
        }

        protected string SerializeObject<T>(T obj) where T : class 
        {
            using (MemoryStream writerStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(writerStream, obj);
                writerStream.Position = 0;
                using (StreamReader readerStream = new StreamReader(writerStream)) 
                {
                    return readerStream.ReadToEnd();
                }
            }
        }

        protected T DeserializeObject<T>(string blob) where T : class
        {
            using (MemoryStream writerStream = new MemoryStream())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(blob);
                writerStream.Write(bytes, 0, bytes.Length);
                writerStream.Position = 0;
                
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                object o = serializer.ReadObject(writerStream);
                return (o as T);
            }
        }
    }
}
