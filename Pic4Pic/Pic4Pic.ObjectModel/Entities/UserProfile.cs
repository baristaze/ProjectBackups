using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public class UserProfile
    {
        protected bool isTestUser;

        [DataMember()]
        public Guid UserId { get; set; }

        [DataMember()]
        public bool IsActive { get; set; }

        [DataMember()]
        public String Username { get; set; }

        [DataMember()]
        public String DisplayName 
        {
            get
            {
                if (this.isTestUser)
                {
                    int trimIndex = this.Username.LastIndexOf("__");
                    if (trimIndex > 0)
                    {
                        return this.Username.Substring(0, trimIndex);
                    }
                }

                return this.Username;
            }
            set
            {
                // void
            }
        }

        [DataMember()]
        public Gender Gender { get; set; }

        [DataMember()]
        public DateTime BirthDay { get; set; }
                
        [DataMember()]
        public string HometownCity { get; set; }

        [DataMember()]
        public string HometownState { get; set; }

        [DataMember()]
        public MaritalStatus MaritalStatus { get; set; }

        [DataMember()]
        public string Profession { get; set; }

        [DataMember()]
        public EducationLevel EducationLevel { get; set; }

        [DataMember()]
        public int TimeZoneOffset { get; set; }

        [DataMember()]
        public DateTime CreateTimeUTC { get; set; }

        [DataMember()]        
        public DateTime LastUpdateTimeUTC { get; set; }

        [DataMember()]
        public string Description { get; set; } // bio

        [DataMember()]
        public string ShortBio
        {
            get
            {
                return this.generateShortBio();
            }
            set
            {
                throw new NotSupportedException("Set ShortBio is not supported. Keeping this for the sake of serialization");
            }
        }

        public UserProfile() 
        {
            this.CreateTimeUTC = DateTime.UtcNow;
            this.LastUpdateTimeUTC = this.CreateTimeUTC;
        }

        public UserProfile(FacebookUser fbUser, string userName, string description, bool isActive, bool isTestUser)
        {
            // downgrade
            this.UserId = fbUser.UserId;
            this.IsActive = isActive;
            this.Username = userName;
            this.Gender = fbUser.Gender;
            this.BirthDay = fbUser.BirthDay;
            this.HometownCity = fbUser.HometownCity;
            this.HometownState = fbUser.HometownState;
            this.MaritalStatus = fbUser.MaritalStatus;
            this.Profession = fbUser.Profession;
            this.EducationLevel = fbUser.EducationLevel;
            this.TimeZoneOffset = fbUser.TimeZoneOffset;
            this.CreateTimeUTC = fbUser.CreateTimeUTC;
            this.LastUpdateTimeUTC = fbUser.LastUpdateTimeUTC;
            this.Description = description == null ? String.Empty : description;
            this.isTestUser = isTestUser;
        }

        public override string ToString()
        {
            return this.DisplayName;
        }

        protected string generateShortBio()
        {
            return this.generateShortBio(false, " / ");
        }

        protected string generateShortBio(bool includeEducationLevel, string delim)
        {
            string bio = "";

            int age = this.calculateAge();
            if (age > 0)
            {
                if (bio.Length > 0)
                {
                    bio += delim;
                }

                bio += age.ToString();
            }

            if (this.Gender != Gender.Unknown)
            {
                if (bio.Length > 0)
                {
                    bio += delim;
                }

                bio += this.Gender.ToString();
            }

            if (this.MaritalStatus != MaritalStatus.Unknown)
            {
                if (bio.Length > 0)
                {
                    bio += delim;
                }

                bio += this.MaritalStatus.ToString();
            }

            if (!String.IsNullOrWhiteSpace(this.HometownCity))
            {
                if (bio.Length > 0)
                {
                    bio += delim;
                }

                bio += this.HometownCity;
            }

            if (!String.IsNullOrWhiteSpace(this.Profession))
            {
                if (bio.Length > 0)
                {
                    bio += delim;
                }

                bio += this.Profession;
            }

            if (includeEducationLevel && this.EducationLevel > EducationLevel.HighSchool)
            {
                if (bio.Length > 0)
                {
                    bio += delim;
                }

                bio += EducationLevelHelper.ToUserFriendlyString(this.EducationLevel);
            }

            return bio;
        }

        protected int calculateAge()
        {
            DateTime now = DateTime.Now;
            int age = now.Year - this.BirthDay.Year;
            if (now.Month < this.BirthDay.Month)
            {
                age--;
            }
            else if (now.Month == this.BirthDay.Month && now.Day < this.BirthDay.Day)
            {
                age--;
            }

            if (age < 0 || age > 120)
            {
                age = 0;
            }

            return age;
        }
    }
}
