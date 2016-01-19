using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Crosspl.ObjectModel;
using Crosspl.Web.Services;

namespace Crosspl.UnitTests
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void TestJsonSerializationSigninRequest()
        {
            SigninRequest request = new SigninRequest();
            request.OAuthProvider = OAuthProvider.Facebook;
            request.OAuthUserId = "5";
            request.OAuthAccessToken = "1234567890";
            request.OAuthExpiresInSeconds = 2 * 60;
            request.SplitId = 10;

            string json = TestJson<SigninRequest>(request, true);
            Console.WriteLine(json);
        }

        [TestMethod]
        public void TestJsonSerializationAuthTokenResponse()
        {
            AuthTokenResponse response = new AuthTokenResponse();
            response.OAuthProvider = OAuthProvider.Facebook;
            response.PhotoUrl = "http://baristaze.com";
            response.Token = "1234567890";
            response.UserFriendlyName = "Baris";
            response.ExpiresInSeconds = 120;

            string json = TestJson<AuthTokenResponse>(response, true);
            Console.WriteLine(json);
        }

        [TestMethod]
        public void TestJsonSerializationEntryRequest()
        {
            EntryRequest request = new EntryRequest();
            request.TopicId = 3;
            request.EntryId = 5;
            request.Content = "foo bar";

            string json = TestJson<EntryRequest>(request, true);
            Console.WriteLine(json);
        }

        [TestMethod]
        public void TestJsonSerializationEntryResponse()
        {
            Entry entry = new Entry();
            entry.Id = 5;
            entry.Content = "foo bar";
            entry.CreatedBy = 1;
            entry.FormatVersion = 1;
            entry.TopicId = 7;

            EntryResponse response = new EntryResponse();
            response.Entry = entry;
            response.Entry.ContentAsEncodedHtml = "<div>click next &gt;&gt;</div>";

            string json = TestJson<EntryResponse>(response, true);
            Console.WriteLine(json);
        }

        private string TestJson<T>(T obj, bool deserializeToo)
        {
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            ser.WriteObject(stream, obj);
            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);
            string jsonData = reader.ReadToEnd();

            if (deserializeToo)
            {
                stream.Position = 0;
                T o2 = (T)ser.ReadObject(stream);
            }

            return jsonData;
        }
    }
}
