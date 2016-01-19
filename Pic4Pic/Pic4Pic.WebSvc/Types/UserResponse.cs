using System;
using System.Runtime.Serialization;

using Pic4Pic.ObjectModel;
using System.Collections.Generic;

namespace Pic4Pic.WebSvc
{   
    [DataContract()]
    public class UserResponse : BaseResponse
    {
        private static readonly string SettingKey_InitialFacebookPermissions = "InitialFacebookPermissions";

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

        public UserResponse() : base () { }

        public UserResponse(int errorCode, string errorMessage) : base(errorCode, errorMessage) { }

        public void AttachSettings(int splitId)
        {
            // add settings...
            if (splitId == 0)
            {
                this.Settings.Add(new NameValue(SettingKey_InitialFacebookPermissions, "email,user_birthday,user_relationships,user_hometown,user_work_history,user_education_history"));
            }
            else if (splitId == 1)
            {
                this.Settings.Add(new NameValue(SettingKey_InitialFacebookPermissions, "email,user_birthday,user_relationships,user_hometown"));
            }
            else // if (splitId == 2)
            {
                this.Settings.Add(new NameValue(SettingKey_InitialFacebookPermissions, "email"));
            }
        }

        public static int GetRandomSplitId()
        {
            Random random = new Random((int)(DateTime.Now.Ticks % Int32.MaxValue));
            int number = random.Next(0, 100);
            if (number < 70) 
            {
                return 0;
            }
            if (number < 90)
            {
                return 1;
            }
            return 2;
        }
    }
}