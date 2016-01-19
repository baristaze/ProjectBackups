using System;
using System.Runtime.Serialization;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WebSvc
{
    [DataContract()]
    public class UserCredentials : BaseRequest
    {
        public const int MIN_USERNAME_LEN = 6;
        public const int MAX_USERNAME_LEN = 20;
        public const int MIN_PASSWORD_LEN = 6;
        public const int MAX_PASSWORD_LEN = 20;
        
        [DataMember()]
        public string Username { get; set; }

        [DataMember()]
        public string Password { get; set; }

        public override string ToString()
        {
            return this.Username;
        }

        public override void Validate()
        {
            this.Validate(false);
        }

        public void Validate(bool ignoreShortUsername) 
        {
            base.Validate();

            if (String.IsNullOrWhiteSpace(this.Username))
            {
                throw new Pic4PicArgumentException("Username is null or empty", "Username");
            }

            this.Username = this.Username.Trim();
            if (this.Username.Length < UserCredentials.MIN_USERNAME_LEN)
            {
                if (ignoreShortUsername && this.Username.Length > 1)
                {
                    ;
                }
                else
                {
                    throw new Pic4PicArgumentException("Username is too short", "Username");
                }
            }

            if (this.Username.Length > UserCredentials.MAX_USERNAME_LEN)
            {
                throw new Pic4PicArgumentException("Username is too long", "Username");
            }

            if (String.IsNullOrWhiteSpace(this.Password))
            {
                throw new Pic4PicArgumentException("Password is null or empty", "Password");
            }

            if (this.Password.Length < UserCredentials.MIN_PASSWORD_LEN)
            {
                throw new Pic4PicArgumentException("Password is too short", "Password");
            }

            if (this.Password.Length > UserCredentials.MAX_PASSWORD_LEN)
            {
                throw new Pic4PicArgumentException("Password is too long", "Password");
            }
        }
    }
}