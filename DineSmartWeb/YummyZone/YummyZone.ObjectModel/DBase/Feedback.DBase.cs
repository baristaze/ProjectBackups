using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public partial class Feedback
    {
        public const int PageSize = 10;

        public static FeedbackList GetRecentFeedbacks(
            SqlConnection connection, Guid venueId, DateTime? tillUTC, Guid currentVenueUser, int timeOut)
        {
            FeedbackList fbkList = new FeedbackList();

            // get checkins            
            CheckinList checkinList = Checkin.GetRecentCheckins(
                connection, venueId, Feedback.PageSize, tillUTC, currentVenueUser, timeOut);

            if (checkinList.Count > 0)
            {
                // get rates on menu items related to the checkins that we are interested
                string menuItemRatesQuery = MenuItemRate.SelectMultipleQuery(checkinList);
                MenuItemRateList allMenuItemRates = Database.SelectAll<MenuItemRate, MenuItemRateList>(
                    connection, null, menuItemRatesQuery, timeOut);

                // get all answers related to the checkins that we are interested
                string answersQuery = Answer.SelectMultipleQuery(checkinList);
                AnswerList allAnswers = Database.SelectAll<Answer, AnswerList>(
                    connection, null, answersQuery, timeOut);

                // prepare feedback list
                foreach (Checkin checkin in checkinList)
                {
                    // prepare feedback
                    Feedback fbk = new Feedback();
                    fbk.CheckInId = checkin.Id;
                    fbk.CheckInTimeUTC = checkin.TimeUTC;
                    fbk.DinerType = Diner.ToString(Diner.ConvertToDinerType(checkin.CheckinCountInTotal));
                    fbk.IsRead = checkin.ReadByManager > 0;
                    fbk.IsReplied = (checkin.SentMessageCount > 0);// || (checkin.SentCouponCount > 0);
                    fbk.IsCouponSent = checkin.SentCouponCount > 0;

                    // fbk.CustomerType = Diner.ToString(Diner.ConvertToCustomerType(checkin.CheckinCountAtVenue));
                    
                    fbk.CustomerType = String.Format(
                            CultureInfo.InvariantCulture, 
                            "{0}/{1} of them {2} with us", 
                            checkin.CheckinCountAtVenue, 
                            checkin.CheckinCountInTotal, 
                            (checkin.CheckinCountAtVenue == 1 ? "is" : "are"));

                    // prepare menu item rates
                    MenuItemRateList filteredMenuItemRates = allMenuItemRates.FilterBy(checkin.Id);
                    foreach (MenuItemRate menuItemRate in filteredMenuItemRates)
                    {
                        fbk.RateItems.Add(menuItemRate.ConvertToNameAndValuePair());
                    }

                    // prepare answers
                    AnswerList filteredAnswers = allAnswers.FilterBy(checkin.Id);

                    // rate items
                    foreach (Answer answer in filteredAnswers.RateAnswers)
                    {
                        fbk.RateItems.Add(answer.ConvertToNameAndValuePair());
                    }

                    // yes no items
                    foreach (Answer answer in filteredAnswers.YesNoAnswers)
                    {
                        fbk.YesNoItems.Add(answer.ConvertToNameAndValuePair());
                    }

                    // multi choice items
                    foreach (Answer answer in filteredAnswers.MultiChoiceAnswers)
                    {
                        fbk.MultiChoiceItems.Add(answer.ConvertToNameAndValuePair());
                    }

                    // free text items
                    foreach (Answer answer in filteredAnswers.FreeFormAnswers)
                    {
                        fbk.FreeFormItems.Add(answer.ConvertToNameAndValuePair());
                    }

                    // does feedback have any info on it?
                    if (fbk.HasAnyComment)
                    {
                        fbkList.Add(fbk);
                    }
                    else
                    {
                        throw new YummyZoneException("Feedback doesn't have any comment");
                    }
                }            
            }

            return fbkList;
        }
    }
}
