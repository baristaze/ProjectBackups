using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.Serialization;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// httphandler
    /// </summary>
    public class RegisterVenue : YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            string venueName = this.GetMandatoryString(context, "venueName", "Restaurant Name", 100, Source.Form);
            if (venueName.IndexOf(';') >= 0)
            {
                throw new YummyZoneArgumentException("Symbol ';' is not allowed in Venue Name");
            }

            string venueAddressLine1 = this.GetMandatoryString(context, "venueAddressLine1", "Restaurant Address", 300, Source.Form);
            string venueAddressLine2 = this.GetString(context, "venueAddressLine2", "Address Line 2", 100, Source.Form);
            string venueAddressCity = this.GetMandatoryString(context, "venueAddressCity", "City", 50, Source.Form);

            if (venueAddressCity.IndexOfAny(Address.ExcludedCharsInCity.ToCharArray()) >= 0)
            {
                throw new YummyZoneArgumentException("No symbol allowed in City");
            }

            string venueAddressState = this.GetMandatoryString(context, "venueAddressState", "State", 2, Source.Form);
            venueAddressState = venueAddressState.ToUpperInvariant();
            if (!StringHelpers.IsValidState(venueAddressState))
            {
                throw new YummyZoneArgumentException("Invalid State in the address");
            }

            string venueAddressZip = this.GetMandatoryString(context, "venueAddressZip", "Zip Code", 10, Source.Form);
            if (!StringHelpers.IsValidZipCode(venueAddressZip))
            {
                throw new YummyZoneArgumentException("Invalid Zip Code");
            }

            string userFirstName = this.GetMandatoryString(context, "userFirstName", "First Name", 100, Source.Form);
            if (userFirstName.IndexOfAny(User.ExcludedCharsInName.ToCharArray()) >= 0)
            {
                throw new YummyZoneArgumentException("No symbol allowed in First Name");
            }

            string userLastName = this.GetMandatoryString(context, "userLastName", "Last Name", 100, Source.Form);
            if (userLastName.IndexOfAny(User.ExcludedCharsInName.ToCharArray()) >= 0)
            {
                throw new YummyZoneArgumentException("No symbol allowed in Last Name");
            }

            string userPhone = this.GetMandatoryString(context, "userPhone", "Phone Number", 20, Source.Form);
            if (!StringHelpers.IsValidPhoneNumber(userPhone))
            {
                throw new YummyZoneArgumentException("Invalid Phone Number");
            }

            string userEmail = this.GetMandatoryString(context, "userEmail", "Email Address", 200, Source.Form, false);
            if (!StringHelpers.IsValidEmail(userEmail))
            {
                throw new YummyZoneArgumentException("Invalid Email Address");
            }
            
            string userPassword = this.GetMandatoryString(context, "userPassword", "Password", 100, Source.Form, false);
            if (userPassword.Length < 6)
            {
                throw new YummyZoneArgumentException("Password is too short.");
            }

            SignupInfo signupInfo = new SignupInfo(Guid.NewGuid());
            signupInfo.VenueName = venueName;
            signupInfo.UserFirstName = userFirstName;
            signupInfo.UserLastName = userLastName;
            signupInfo.UserPhoneNumber = userPhone;
            signupInfo.UserEmailAddress = userEmail;
            signupInfo.UserPassword = userPassword;

            signupInfo.Address = new Address();
            signupInfo.Address.ObjectType = ObjectType.SignupVenue;
            signupInfo.Address.ObjectId = signupInfo.Id;
            signupInfo.Address.AddressType = AddressType.BusinessAddress;

            signupInfo.Address.AddressLine1 = venueAddressLine1;
            signupInfo.Address.AddressLine2 = venueAddressLine2;
            signupInfo.Address.City = venueAddressCity;
            signupInfo.Address.State = venueAddressState;
            signupInfo.Address.ZipCode = venueAddressZip;

            IEditable[] entities = new IEditable[] { signupInfo, signupInfo.Address };

            Database.InsertOrUpdate(entities, Helpers.ConnectionString);

            try
            {
                this.SendMail(signupInfo);
            }
            catch { }

            return signupInfo.Id.ToString("N");
        }

        private void SendMail(SignupInfo signupInfo)
        {
            MailSettings settings = new MailSettings();

            settings.SmtpHost = Helpers.EMail_SmtpHost;
            settings.SmtpPort = Helpers.EMail_SmtpPort;
            settings.SenderEmail = Helpers.EMail_SenderEmail;
            settings.SenderName = Helpers.EMail_SenderName;
            settings.SenderPswd = Helpers.EMail_SenderPswd;
            settings.UseSSL = Helpers.EMail_UseSSL;

            MailSender sender = new MailSender(settings);

            sender.TrySend(
                signupInfo.ToSubjectString(), 
                signupInfo.ToContentString("<br/>"),
                Helpers.EMail_ToList,
                Helpers.EMail_CCList,
                Helpers.EMail_BCCList);
        }
    }
}