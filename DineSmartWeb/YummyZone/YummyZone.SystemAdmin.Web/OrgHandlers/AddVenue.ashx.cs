using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.SystemAdmin.Web
{
    /// <summary>
    /// AddVenue httphandler
    /// </summary>
    public class AddVenue : YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            SystemUser user = LoginHelper.GetIdentityFromAuth(context.Request, true);

            string newChainName = string.Empty;
            string newGroupName = string.Empty;
            Venue venue = this.GetVenueFromInputForm(context, ref newChainName, ref newGroupName);

            bool newChainIsBeingAdded = false;
            List<IEditable> entities = new List<IEditable>();
            if (venue.GroupId == Guid.Empty)
            {
                Group group = new Group();
                group.Id = Guid.NewGuid();
                group.Name = newGroupName;
                venue.GroupId = group.Id;
                entities.Add(group);
            }

            if (venue.ChainId == Guid.Empty)
            {
                Chain chain = new Chain();
                chain.Name = newChainName;
                chain.GroupId = venue.GroupId;
                venue.ChainId = chain.Id;
                entities.Add(chain);
                newChainIsBeingAdded = true;
            }

            entities.Add(venue);
            entities.Add(venue.Address);

            List<IEditable> defaultMenus = this.CreateDefaultMenuList(venue.GroupId, venue.Id);
            entities.AddRange(defaultMenus);

            if (newChainIsBeingAdded)
            {
                List<IEditable> defaultSurveyData = this.CreateDefaultSurvey(venue.GroupId, venue.ChainId, venue.Id);
                entities.AddRange(defaultSurveyData);
            }

            Database.InsertOrUpdate(entities, Helpers.ConnectionString);

            /*
            try
            {
                this.SendMail(venue);
            }
            catch { }
            */ 

            return venue.Id.ToString("N");
        }

        private Venue GetVenueFromInputForm(HttpContext context, ref string newChainName, ref string newGroupName)
        {
            string venueName = this.GetMandatoryString(context, "venueName", "Restaurant Name", 100, Source.Form);
            if (venueName.IndexOf(';') >= 0)
            {
                throw new YummyZoneArgumentException("Symbol ';' is not allowed in Venue Name");
            }

            venueName = StringHelpers.ToTitleCase(venueName);

            string venueUrl = this.GetString(context, "venueUrl", "Web Site", 1000, Source.Form);
            if (!String.IsNullOrWhiteSpace(venueUrl))
            {
                venueUrl = venueUrl.ToLower();
            }

            string venueAddressLine1 = this.GetMandatoryString(context, "venueAddressLine1", "Restaurant Address", 300, Source.Form);
            venueAddressLine1 = venueAddressLine1.Replace(" Avenue ", " Ave ");
            venueAddressLine1 = venueAddressLine1.Replace(" Street ", " St ");

            string venueAddressLine2 = this.GetString(context, "venueAddressLine2", "Address Line 2", 100, Source.Form);
            string venueAddressCity = this.GetMandatoryString(context, "venueAddressCity", "City", 50, Source.Form);
            
            if (venueAddressCity.IndexOfAny(Address.ExcludedCharsInCity.ToCharArray()) >= 0)
            {
                throw new YummyZoneArgumentException("No symbol allowed in City");
            }

            venueAddressCity = StringHelpers.ToTitleCase(venueAddressCity);

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

            byte timeZoneIndex = (byte)this.GetMandatoryInt(context, "venueTimeZoneIndex", "Time Zone", 0, 255, Source.Form);
            ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
            List<int> supportedTimeZones = Helpers.SupportedTimeZoneIndices(timeZones.Count - 1);
            if (!supportedTimeZones.Contains(timeZoneIndex))
            {
                throw new YummyZoneArgumentException("Not Supported Time Zone");
            }

            decimal latitude = this.GetMandatoryDecimal(context, "latitude", "Latitude", (decimal)-90.0, (decimal)+90.0, Source.Form);
            decimal longtitude = this.GetMandatoryDecimal(context, "longtitude", "Longtitude", (decimal)-180.0, (decimal)+180.0, Source.Form);

            string chainName = string.Empty;
            string groupName = string.Empty;
            Guid chainId = this.GetGuid(context, "chainId", "Chain Id", Source.Form);
            Guid groupId = this.GetGuid(context, "groupId", "Group Id", Source.Form);

            if (chainId == Guid.Empty)
            {
                chainName = this.GetString(context, "chainName", "Chain Name", 100, Source.Form);
                if (String.IsNullOrWhiteSpace(chainName))
                {
                    chainName = venueName;
                }

                chainName = StringHelpers.ToTitleCase(chainName);

                if (groupId == Guid.Empty)
                {
                    groupName = this.GetString(context, "groupName", "Group Name", 100, Source.Form);
                    if (String.IsNullOrWhiteSpace(groupName))
                    {
                        groupName = chainName;
                    }

                    groupName = StringHelpers.ToTitleCase(groupName);
                }
            }
            else
            {
                if (groupId == Guid.Empty)
                {
                    Chain chain = this.GetChain(chainId);
                    if (chain == null)
                    {
                        throw new YummyZoneException("Unknown Chain");
                    }
                    else
                    {
                        groupId = chain.GroupId;
                    }
                }
            }

            Venue venue = new Venue();
            venue.Name = venueName;
            venue.ChainId = chainId;
            venue.GroupId = groupId;
            venue.WebURL = venueUrl;
            venue.Latitude = latitude;
            venue.Longitude = longtitude;
            venue.TimeZoneWinIndex = timeZoneIndex;
            venue.Status = VenueStatus.Draft;

            Address address = new Address();
            address.ObjectType = ObjectType.Venue;
            address.ObjectId = venue.Id;
            address.AddressType = AddressType.BusinessAddress;
            address.AddressLine1 = venueAddressLine1;
            address.AddressLine2 = venueAddressLine2;
            address.City = venueAddressCity;
            address.State = venueAddressState;
            address.ZipCode = venueAddressZip;
            venue.Address = address;

            newChainName = chainName;
            newGroupName = groupName;

            return venue;
        }

        private Chain GetChain(Guid chainId)
        {
            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = Chain.SelectQuery(chainId);
                    command.CommandTimeout = Database.TimeoutSecs;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Chain chain = new Chain();
                            chain.InitFromSqlReader(reader);
                            return chain;
                        }
                    }
                }
            }

            return null;
        }

        private List<IEditable> CreateDefaultMenuList(Guid groupId, Guid venueId)
        {
            List<IEditable> items = new List<IEditable>();

            // add default menus
            Menu dinner = new Menu(groupId, "Dinner", 990, 1410);
            Menu lunch = new Menu(groupId, "Lunch", 690, 990);
            Menu drinks = new Menu(groupId, "Drinks", 690, 1410);
            Menu desserts = new Menu(groupId, "Desserts", 690, 1410);

            items.Add(dinner);
            items.Add(lunch);
            items.Add(drinks);
            items.Add(desserts);

            // attach menus to the venue
            MapVenueToMenu mapDinner = new MapVenueToMenu(groupId, venueId, dinner.Id, 0);
            MapVenueToMenu mapLunch = new MapVenueToMenu(groupId, venueId, lunch.Id, 1);
            MapVenueToMenu mapDrinks = new MapVenueToMenu(groupId, venueId, drinks.Id, 2);
            MapVenueToMenu mapDesserts = new MapVenueToMenu(groupId, venueId, desserts.Id, 3);

            items.Add(mapDinner);
            items.Add(mapLunch);
            items.Add(mapDrinks);
            items.Add(mapDesserts);

            // add default categories
            MenuCategory appetizers = new MenuCategory(groupId, "Appetizers");
            MenuCategory soupsAndSalad = new MenuCategory(groupId, "Soups and Salads");
            MenuCategory entrees = new MenuCategory(groupId, "Entrées");
            MenuCategory drinksDefault = new MenuCategory(groupId, "Wine");
            MenuCategory dessertsDefault = new MenuCategory(groupId, "Desserts");

            items.Add(appetizers);
            items.Add(soupsAndSalad);
            items.Add(entrees);
            items.Add(drinksDefault);
            items.Add(dessertsDefault);

            // attach categories to menus
            MapMenuToCategory dinnerAppetizers = new MapMenuToCategory(groupId, dinner.Id, appetizers.Id, 0);
            MapMenuToCategory dinnerSoupAndSalads = new MapMenuToCategory(groupId, dinner.Id, soupsAndSalad.Id, 1);
            MapMenuToCategory dinnerEntrees = new MapMenuToCategory(groupId, dinner.Id, entrees.Id, 2);
            MapMenuToCategory def1 = new MapMenuToCategory(groupId, drinks.Id, drinksDefault.Id, 0);
            MapMenuToCategory def2 = new MapMenuToCategory(groupId, desserts.Id, dessertsDefault.Id, 0);

            items.Add(dinnerAppetizers);
            items.Add(dinnerSoupAndSalads);
            items.Add(dinnerEntrees);
            items.Add(def1);
            items.Add(def2);

            return items;
        }

        private List<IEditable> CreateDefaultSurvey(Guid groupId, Guid chainId, Guid venueId)
        {
            List<IEditable> items = new List<IEditable>();
            
            Question serviceAndStaff = new Question();
            serviceAndStaff.GroupId = groupId;
            serviceAndStaff.QuestionType = QuestionType.Rate;
            serviceAndStaff.Wording = "Service & Staff";
            items.Add(serviceAndStaff);
            items.Add(new MapChainToQuestion(groupId, chainId, serviceAndStaff.Id, 1));
            
            Question cleanliness = new Question();
            cleanliness.GroupId = groupId;
            cleanliness.QuestionType = QuestionType.Rate;
            cleanliness.Wording = "Cleanliness";
            items.Add(cleanliness);
            items.Add(new MapChainToQuestion(groupId, chainId, cleanliness.Id, 2));
            
            Question atmosphere = new Question();
            atmosphere.GroupId = groupId;
            atmosphere.QuestionType = QuestionType.Rate;
            atmosphere.Wording = "Atmosphere";
            items.Add(atmosphere);
            items.Add(new MapChainToQuestion(groupId, chainId, atmosphere.Id, 3));
            
            Question satisfaction = new Question();
            satisfaction.GroupId = groupId;
            satisfaction.QuestionType = QuestionType.Rate;
            satisfaction.Wording = "Overall Satisfaction";
            items.Add(satisfaction);
            items.Add(new MapChainToQuestion(groupId, chainId, satisfaction.Id, 4));
            
            Question recommendToFriends = new Question();
            recommendToFriends.GroupId = groupId;
            recommendToFriends.QuestionType = QuestionType.YesNo;
            recommendToFriends.Wording = "Would you consider recommending us to your friends?";
            items.Add(recommendToFriends);
            items.Add(new MapChainToQuestion(groupId, chainId, recommendToFriends.Id, 5));
                        
            // > multiple choice - begin
            Question likelihoodOfComingBack = new Question();
            likelihoodOfComingBack.GroupId = groupId;
            likelihoodOfComingBack.QuestionType = QuestionType.MultiChoice;
            likelihoodOfComingBack.Wording = "How likely is it that you would come back to our restaurant again?";
            items.Add(likelihoodOfComingBack);
            items.Add(new MapChainToQuestion(groupId, chainId, likelihoodOfComingBack.Id, 6));

            Choice verlyLikely = new Choice(groupId, "Very Likely");
            Choice likely = new Choice(groupId, "Likely");
            Choice maybe = new Choice(groupId, "Maybe");
            Choice unlikely = new Choice(groupId, "Unlikely");
            Choice verlyUnlikely = new Choice(groupId, "Very Unlikely");

            items.Add(verlyLikely);
            items.Add(likely);
            items.Add(maybe);
            items.Add(unlikely);
            items.Add(verlyUnlikely);

            items.Add(new MapQuestionToChoice(groupId, likelihoodOfComingBack.Id, verlyLikely.Id, 1));
            items.Add(new MapQuestionToChoice(groupId, likelihoodOfComingBack.Id, likely.Id, 2));
            items.Add(new MapQuestionToChoice(groupId, likelihoodOfComingBack.Id, maybe.Id, 3));
            items.Add(new MapQuestionToChoice(groupId, likelihoodOfComingBack.Id, unlikely.Id, 4));
            items.Add(new MapQuestionToChoice(groupId, likelihoodOfComingBack.Id, verlyUnlikely.Id, 5));

            // < multiple choice - end

            Question otherComments = new Question();
            otherComments.GroupId = groupId;
            otherComments.QuestionType = QuestionType.FreeText;
            otherComments.Wording = "Other comments?";
            items.Add(otherComments);
            items.Add(new MapChainToQuestion(groupId, chainId, otherComments.Id, 7));
            
            return items;
        }

        private void SendMail(Venue venue)
        {
            MailSettings settings = new MailSettings();

            settings.SmtpHost = Helpers.EMail_SmtpHost;
            settings.SmtpPort = Helpers.EMail_SmtpPort;
            settings.SenderEmail = Helpers.EMail_SenderEmail;
            settings.SenderName = Helpers.EMail_SenderName;
            settings.SenderPswd = Helpers.EMail_SenderPswd;
            settings.UseSSL = Helpers.EMail_UseSSL;

            MailSender sender = new MailSender(settings);

            string content = String.Empty;
            content += "Name: " + venue.Name;

            if(!String.IsNullOrWhiteSpace(venue.WebURL))
            {
                content += "<br/>";
                content += "WebURL: " + venue.WebURL;
            }

            if(venue.Address != null)
            {
                content += "<br/>";
                content += "Address: " + venue.Address.ToString();
            }

            sender.TrySend(
                "Draft venue added: " + venue.Name,
                content,
                Helpers.EMail_ToList,
                Helpers.EMail_CCList,
                Helpers.EMail_BCCList);
        }
    }
}