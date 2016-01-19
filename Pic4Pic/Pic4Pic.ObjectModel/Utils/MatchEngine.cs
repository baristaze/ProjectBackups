using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public class MatchConfig
    {
        public static int RematchLimitAsDays = 14;

        public int MinMatchToShow { get; set; }
        public int MaxMatchToShow { get; set; }
        public int MinFreshMatch { get; set; }
        public int FreshnessAsMinutes { get; set; }
        public int HistoryLimitAsMinutes { get; set; }
        public int RematchLimitAsMinutes { get; set; }

        public MatchConfig()
        {
            this.MinMatchToShow = 8;
            this.MaxMatchToShow = 30;
            this.MinFreshMatch = 5;
            this.FreshnessAsMinutes = 24 * 60;
            this.HistoryLimitAsMinutes = 90 * 24 * 60; // 90 days
            this.RematchLimitAsMinutes = MatchConfig.RematchLimitAsDays * 24 * 60; // 14 days
        }

        public static MatchConfig CreateDefault()
        {
            return new MatchConfig();
        }
    }

    public class MatchEngine
    {
        public static List<MatchedCandidate> PrepareTodaysMatches(
            SqlConnection conn,
            SqlTransaction trans,
            Guid userId,
            MatchConfig config,
            string homeTownState,
            string concatenatedCitiesInRange)
        {
            // get matches
            Dictionary<Guid, DateTime> lastLikeTimes = new Dictionary<Guid, DateTime>();
            Dictionary<Guid, DateTime> lastViewTimes = new Dictionary<Guid, DateTime>(config.MaxMatchToShow);
            List<FacebookUser> matchesAsFacebookUsers = MatchEngine.GetTodaysMatches(
                conn, trans, userId, config, ref lastViewTimes, ref lastLikeTimes, homeTownState, concatenatedCitiesInRange);
            if (matchesAsFacebookUsers.Count <= 0)
            {
                return new List<MatchedCandidate>();
            }

            // bind details
            List<MatchedCandidate> candidates = MatchEngine.BindDetailsToMatches(conn, trans, userId, matchesAsFacebookUsers);

            // bind further details for old matches
            foreach (MatchedCandidate candidate in candidates)
            {
                if (lastViewTimes.ContainsKey(candidate.CandidateProfile.UserId))
                {
                    candidate.LastViewTimeUTC = lastViewTimes[candidate.CandidateProfile.UserId];
                }

                if (lastLikeTimes.ContainsKey(candidate.CandidateProfile.UserId))
                {
                    candidate.LastLikeTimeUTC = lastLikeTimes[candidate.CandidateProfile.UserId];
                }
            }

            // return
            return candidates;
        }

        public static List<MatchedCandidate> BuyNewMatches(
            SqlConnection conn,
            SqlTransaction trans,
            Guid userId,
            int maxMatchCount,
            int rematchLimitAsMinutes,
            string homeTownState,
            string concatenatedCitiesInRange)
        {
            // make new recommendations
            List<FacebookUser> newMatches = Recommendation.MakeNewRecommendations(
                conn, trans, userId, maxMatchCount, rematchLimitAsMinutes, homeTownState, concatenatedCitiesInRange);

            if (newMatches.Count <= 0)
            {
                return new List<MatchedCandidate>();
            }

            return MatchEngine.BindDetailsToMatches(conn, trans, userId, newMatches);
        }

        public static List<MatchedCandidate> GetPreviewMatches(
            SqlConnection conn,
            SqlTransaction trans,
            int maxMatchCount,
            string homeTownState,
            string concatenatedCitiesInRange)
        {
            // make new recommendations
            List<FacebookUser> newMatches = Recommendation.GetPreviewRecommendations(
                conn, trans, maxMatchCount, homeTownState, concatenatedCitiesInRange);

            if (newMatches.Count <= 0)
            {
                return new List<MatchedCandidate>();
            }

            // a random guid will make everything non-familiar & no-interaction yet
            return MatchEngine.BindDetailsToMatches(conn, trans, Guid.NewGuid(), newMatches);
        }

        public static MatchedCandidate GetMatchByUserId(
            SqlConnection conn,
            SqlTransaction trans,
            Guid currentUserId,
            Guid matchedUserId,
            Familiarity familiarity,
            bool ignoreLastLikeTime,
            MatchConfig config)
        {
            if (currentUserId == matchedUserId)
            {
                throw new Pic4PicException("User ID may not be equal to candidate user ID. (Internal error)");
            }

            FacebookUser fbUser = FacebookUser.ReadFromDBase(conn, trans, matchedUserId);
            return MatchEngine.BindDetailsToMatch(conn, trans, currentUserId, fbUser, familiarity, ignoreLastLikeTime, config);
        }

        private static MatchedCandidate BindDetailsToMatch(
            SqlConnection conn,
            SqlTransaction trans,
            Guid currentUserId,
            FacebookUser fbMatchedUser,
            Familiarity familiarity,
            bool ignoreLastLikeTime,
            MatchConfig config)
        {
            // get user since we need username
            User user = User.ReadFromDBaseById(conn, trans, fbMatchedUser.UserId);

            if (fbMatchedUser.UserId == User.WellKnownSystemUserId) 
            {
                familiarity = Familiarity.Familiar;
            }

            // create friend profile
            FriendProfile profile = new FriendProfile(fbMatchedUser, user.Username, user.Description, familiarity, (user.UserType == UserType.Guest));

            // retrieve images for matched user
            List<ImageFile> images = ImageFile.ReadAllFromDBaseByUserId(conn, trans, fbMatchedUser.UserId);

            // retrive pic4pic's for the users
            Dictionary<Guid, List<PicForPic>> pic4picMaps = PicForPic.ReadAllFromDBaseForMultipleUsers(
                conn, trans, currentUserId, fbMatchedUser.UserId.ToString(), 100);

            // create candidate response
            MatchedCandidate candidate = new MatchedCandidate();
            candidate.CandidateProfile = profile;

            // get profile pics
            PicturePair profilePics = PicturePair.GetProfilePicturePair(images, familiarity);
            if (profilePics.HasAll())
            {
                // set profile pics
                candidate.ProfilePics = profilePics;

                // add other images
                if (pic4picMaps.ContainsKey(fbMatchedUser.UserId))
                {
                    // add sent and received p4p requests...
                    Pic4PicHistory history = Pic4PicHistory.From(pic4picMaps[fbMatchedUser.UserId], currentUserId, fbMatchedUser.UserId);
                    candidate.SentPic4PicsByCandidate.AddRange(history.SentByCandidate);
                    candidate.SentPic4PicsToCandidate.AddRange(history.SentToCandidate);

                    // add other pictures
                    List<PicturePair> otherImages = PicturePair.GetNonProfilePicturePairs(images, pic4picMaps[fbMatchedUser.UserId]);
                    candidate.OtherPictures.AddRange(otherImages);
                }
                else
                {
                    // add other pictures
                    List<PicturePair> otherImages = PicturePair.GetNonProfilePicturePairs(images, new List<PicForPic>());
                    candidate.OtherPictures.AddRange(otherImages);
                }
            }

            // bind flag to indicate whether we liked this candidate before
            if (!ignoreLastLikeTime)
            {
                // bind likes-by-me info 
                List<Action> myLikesOfMatcherUser = Action.ReadAllByMeFromDBase(conn, trans, currentUserId, fbMatchedUser.UserId.ToString(), 1, config.HistoryLimitAsMinutes, ActionType.LikedBio);
                if (myLikesOfMatcherUser.Count > 0)
                {
                    Action like = myLikesOfMatcherUser[0];
                    candidate.LastLikeTimeUTC = like.ActionTimeUTC;
                }
            }

            // return
            return candidate;
        }

        private static List<MatchedCandidate> BindDetailsToMatches(
            SqlConnection conn, SqlTransaction trans, Guid userId, List<FacebookUser> matchesAsFacebookUsers)
        {
            // check input
            if (matchesAsFacebookUsers.Count <= 0)
            {
                return new List<MatchedCandidate>();
            }

            // we need to retrieve users since we need usernames (nicknames)
            Dictionary<Guid, Familiarity> familiarityMaps = new Dictionary<Guid, Familiarity>();
            string concatenatedUserIds = Pic4PicUtils.ConcatenateIDs<FacebookUser>(matchesAsFacebookUsers, (m) => { return m.UserId; });
            List<User> matchesAsUsers = User.ReadAllFromDBaseWithFamiliaritiesAndHash(conn, trans, concatenatedUserIds, userId, ref familiarityMaps);

            // keep uses in a map
            Dictionary<Guid, User> userMaps = new Dictionary<Guid, User>();
            for (int x = 0; x < matchesAsUsers.Count; x++)
            {
                // build the map
                User user = matchesAsUsers[x];
                userMaps.Add(user.Id, user);
            }

            // now, create friend profiles from FB+User
            List<FriendProfile> matchesAsFriendProfiles = new List<FriendProfile>();
            foreach (FacebookUser fbUser in matchesAsFacebookUsers)
            {
                User user = userMaps[fbUser.UserId];
                Familiarity familiarity = familiarityMaps[fbUser.UserId];
                if (fbUser.UserId == User.WellKnownSystemUserId)
                {
                    familiarity = Familiarity.Familiar;
                }
                FriendProfile profile = new FriendProfile(fbUser, user.Username, user.Description, familiarity, (user.UserType == UserType.Guest));
                matchesAsFriendProfiles.Add(profile);
            }

            // now retrieve images for matched users
            Dictionary<Guid, List<ImageFile>> imageMaps = ImageFile.ReadAllFromDBaseByUserIDsAndHash(conn, trans, concatenatedUserIds);

            // retrive pic4pic's for the users
            Dictionary<Guid, List<PicForPic>> pic4picMaps = PicForPic.ReadAllFromDBaseForMultipleUsers(
                conn, trans, userId, concatenatedUserIds, (matchesAsFacebookUsers.Count * 100));

            // now convert everything to MatchedCandidate
            List<MatchedCandidate> matches = new List<MatchedCandidate>();
            foreach (FriendProfile candidateProfile in matchesAsFriendProfiles)
            {
                // create
                MatchedCandidate candidate = new MatchedCandidate();
                candidate.CandidateProfile = candidateProfile;
                if (imageMaps.ContainsKey(candidateProfile.UserId))
                {
                    List<ImageFile> imagesOfCandidate = imageMaps[candidateProfile.UserId];
                    if (imagesOfCandidate.Count >= 4)
                    {
                        // add pics
                        candidate.ProfilePics = PicturePair.GetProfilePicturePair(imageMaps[candidateProfile.UserId], candidateProfile.Familiarity);
                        if (candidate.ProfilePics.HasAll())
                        {
                            if (pic4picMaps.ContainsKey(candidateProfile.UserId)) 
                            {
                                // add sent and received p4p requests...
                                Pic4PicHistory history = Pic4PicHistory.From(pic4picMaps[candidateProfile.UserId], userId, candidateProfile.UserId);
                                candidate.SentPic4PicsByCandidate.AddRange(history.SentByCandidate);
                                candidate.SentPic4PicsToCandidate.AddRange(history.SentToCandidate);

                                // add other pictures
                                List<PicturePair> otherImages = PicturePair.GetNonProfilePicturePairs(
                                    imageMaps[candidateProfile.UserId], pic4picMaps[candidateProfile.UserId]);

                                // add other pictures
                                candidate.OtherPictures.AddRange(otherImages);
                            }
                            else
                            {
                                // add other pictures
                                List<PicturePair> otherImages = PicturePair.GetNonProfilePicturePairs(imageMaps[candidateProfile.UserId], new List<PicForPic>());
                                candidate.OtherPictures.AddRange(otherImages);
                            }

                            // add candidate
                            matches.Add(candidate);
                        }
                    }
                }
            }

            // return 
            return matches;
        }

        private static List<FacebookUser> GetTodaysMatches(
            SqlConnection conn,
            SqlTransaction trans,
            Guid userId,
            MatchConfig config,
            ref Dictionary<Guid, DateTime> lastViewTimes,
            ref Dictionary<Guid, DateTime> lastLikeTimes,
            string homeTownState,
            string concatenatedCitiesInRange)
        {
            // how many fresh is needed?
            int minFreshMatch = config.MinFreshMatch;

            // get my recent matches... go back till 90 days... fetch only first 100 of them... recents first (orderby)
            List<DateTime> recommendationUtcTimes = new List<DateTime>();
            List<FacebookUser> matches = Recommendation.GetRecentRecommendations_FromDBase(
                conn, trans, userId, config.MaxMatchToShow, config.HistoryLimitAsMinutes, ref recommendationUtcTimes);

            // do I have any match till now?
            if (matches.Count < config.MinMatchToShow)
            {
                // if I don't have any match till now, my freshness definition changes... first-time users gets more
                minFreshMatch = Math.Max(minFreshMatch, (config.MinMatchToShow - matches.Count));
            }

            // calculate fresh matches
            int freshMatchCount = 0;
            DateTime freshCutOffTime = DateTime.UtcNow.AddMinutes(-1 * config.FreshnessAsMinutes);
            foreach (DateTime matchTimeUTC in recommendationUtcTimes)
            {
                if (matchTimeUTC >= freshCutOffTime)
                {
                    freshMatchCount++;
                }
            }

            // has user viewed these matches already?
            string concatenatedUserIds = Pic4PicUtils.ConcatenateIDs<FacebookUser>(matches, (m) => { return m.UserId; });
            if (matches.Count > 0)
            {
                // get my recent actions... 
                // any action (e.g. Poke, requestP4P) implies that I have viewed this profile.
                // therefore do not filter by action type.
                List<Action> myActions = Action.ReadAllByMeFromDBase(
                conn, null, userId, concatenatedUserIds, 300, config.HistoryLimitAsMinutes, ActionType.Undefined); // undefined means all

                // put them into a dictionary by UserId2 for better search
                foreach (Action myAction in myActions)
                {
                    // is viewed?
                    if (!lastViewTimes.ContainsKey(myAction.UserId2))
                    {
                        lastViewTimes.Add(myAction.UserId2, myAction.ActionTimeUTC);
                    }
                    else
                    {
                        if (lastViewTimes[myAction.UserId2] < myAction.ActionTimeUTC)
                        {
                            lastViewTimes[myAction.UserId2] = myAction.ActionTimeUTC;
                        }
                    }

                    // is liked ?
                    if (myAction.ActionType == ActionType.LikedBio)
                    {
                        if (!lastLikeTimes.ContainsKey(myAction.UserId2))
                        {
                            lastLikeTimes.Add(myAction.UserId2, myAction.ActionTimeUTC);
                        }
                        else
                        {
                            if (lastLikeTimes[myAction.UserId2] < myAction.ActionTimeUTC)
                            {
                                lastLikeTimes[myAction.UserId2] = myAction.ActionTimeUTC;
                            }
                        }
                    }
                }
            }

            // do I need fresh matches ?
            int requiredNewMatchCount = Math.Max(0, minFreshMatch - freshMatchCount); // 5 - ?
            if (requiredNewMatchCount > 0)
            {
                // make new recommendations
                List<FacebookUser> newMatches = Recommendation.MakeNewRecommendations(
                    conn, trans, userId, requiredNewMatchCount, config.RematchLimitAsMinutes, homeTownState, concatenatedCitiesInRange);

                // check new match size
                if (newMatches.Count > 0)
                {
                    // make sure that we remove re-matched users from old-matches list so that user thinks that it is new
                    if (matches.Count > 0)
                    {
                        matches = MatchEngine.RemoveExistingMatches(matches, newMatches);
                    }

                    // add them to the list                
                    matches.InsertRange(0, newMatches); // it is safe even if matches is empty.
                }
            }

            // trim
            if (matches.Count > config.MaxMatchToShow)
            {
                matches.RemoveRange(config.MaxMatchToShow, (matches.Count - config.MaxMatchToShow));
            }

            // return
            return matches;
        }

        private static List<FacebookUser> RemoveExistingMatches(List<FacebookUser> oldMatches, List<FacebookUser> newMatches)
        {
            // put old ones into a dictionary to make the search faster.
            Dictionary<Guid, FacebookUser> map = new Dictionary<Guid, FacebookUser>(oldMatches.Count);
            foreach (FacebookUser match in oldMatches)
            {
                map.Add(match.UserId, match);
            }

            // check new matches to see if they show up in the old list
            foreach (FacebookUser newMatche in newMatches)
            {
                if (map.ContainsKey(newMatche.UserId))
                {
                    // remove this so that new-addition can look fresh
                    FacebookUser oldMatch = map[newMatche.UserId];
                    map.Remove(newMatche.UserId);
                    oldMatches.Remove(oldMatch);
                }
            }

            return oldMatches;
        }

        private static bool IsLocationUSA(Locality loc)
        {
            if (loc == null) 
            {
                return false;
            }

            if (!String.IsNullOrWhiteSpace(loc.CountryCode))
            {
                string countryCode = loc.CountryCode.Trim().ToLower();
                if (countryCode == "us" || countryCode == "usa")
                {
                    return true;
                }
            }

            if (!String.IsNullOrWhiteSpace(loc.Country))
            {
                string country = loc.Country.Trim().ToLower();
                if (country == "us" || country == "usa" || country == "united states" || country == "united states of america")
                {
                    return true;
                }
            }

            return false;
        }

        private static Region GetVerifiedRegion(SqlConnection conn, SqlTransaction trans, Locality loc)
        {
            if (loc == null)
            {
                return null;
            }

            if (String.IsNullOrWhiteSpace(loc.Region))
            {
                return null;
            }

            int usCountryId = 1;
            string locRegionName = loc.Region.Trim().ToLower();
            List<Region> regions = Region.ReadAllFromDBase(conn, null, usCountryId);
            Dictionary<string, Region> regionMap = Region.MapItemsByNameAndCode(regions, true, true, true);
            if (!regionMap.ContainsKey(locRegionName))
            {
                return null;
            }

            Region region = regionMap[locRegionName];
            return region;
        }

        private static City GetVerifiedCityBySubRegion(SqlConnection conn, SqlTransaction trans, Locality loc, Region region, out List<City> cities)
        {
            int usCountryId = 1;
            cities = new List<City>();

            if (loc == null || region == null || String.IsNullOrWhiteSpace(loc.City) || String.IsNullOrWhiteSpace(loc.SubRegion))
            {
                return null;
            }

            string locCityName = loc.City.Trim().ToLower();
            string locSubRegionName = loc.SubRegion.Trim().ToLower();
            List<SubRegion> subRegions = SubRegion.ReadAllFromDBase(conn, null, usCountryId, region.Id);
            Dictionary<string, SubRegion> subRegionMap = SubRegion.MapItemsByNameAndCode(subRegions, true, true, true);
            if (!subRegionMap.ContainsKey(locSubRegionName))
            {
                return null;
            }

            // read cities by state & county (sub-region)
            SubRegion subRegion = subRegionMap[locSubRegionName];
            cities = City.ReadAllFromDBase(conn, null, usCountryId, region.Id, subRegion.Id);

            // find city
            Dictionary<string, City> cityMap = City.MapItemsByNameAndCode(cities, true, true, true);
            if (!cityMap.ContainsKey(locCityName))
            {
                return null;
            }

            City theCity = cityMap[locCityName];
            return theCity;
        }

        private static City GetVerifiedCityByRegion(SqlConnection conn, SqlTransaction trans, Locality loc, Region region)
        {
            if (loc == null || region == null || String.IsNullOrWhiteSpace(loc.City))
            {
                return null;
            }

            // read cities by just state
            int usCountryId = 1;
            List<City> cities = City.ReadAllFromDBase(conn, null, usCountryId, region.Id);

            string locCityName = loc.City.Trim().ToLower();
            Dictionary<string, City> cityMap = City.MapItemsByNameAndCode(cities, true, true, true);
            if (!cityMap.ContainsKey(locCityName))
            {
                return null;
            }

            City theCity = cityMap[locCityName];
            return theCity;
        }

        public static string GetCachedStateAndCitiesInRangeOrRead(SqlConnection conn, SqlTransaction trans, Guid userId, Locality loc, ref string concatenatedCitiesInRange)
        {
            // determine home-town city and state....
            string hometownState = null; // can be null, which is fine.
            concatenatedCitiesInRange = null; // can be null, which is fine

            // prepare cache keys
            string cacheKeyState = String.Format(CultureInfo.InvariantCulture, "current_loc_state_for_{0}", userId);
            string cacheKeyCities = String.Format(CultureInfo.InvariantCulture, "current_loc_cities_for_{0}", userId);

            // try to read them from cache
            hometownState = CacheHelper.GetObjOrDefault<String>(CacheHelper.CacheName_Default, cacheKeyState);
            concatenatedCitiesInRange = CacheHelper.GetObjOrDefault<String>(CacheHelper.CacheName_Default, cacheKeyCities);

            // did we able to read from cache?
            if (String.IsNullOrWhiteSpace(hometownState) || String.IsNullOrWhiteSpace(concatenatedCitiesInRange))
            {
                // get it from database
                concatenatedCitiesInRange = null;
                hometownState = MatchEngine.GetStateAndCitiesInRangeByLocality(
                    conn, null, loc, ref concatenatedCitiesInRange);

                if (String.IsNullOrWhiteSpace(hometownState))
                {
                    hometownState = "null";
                }

                if (String.IsNullOrWhiteSpace(concatenatedCitiesInRange))
                {
                    concatenatedCitiesInRange = "null";
                }

                // log
                Logger.bag()
                    .Add("action", "GetCachedStateAndCitiesInRangeOrRead")
                    .Add("state", hometownState)
                    .Add("cities", concatenatedCitiesInRange)
                    .LogInfo();

                // update the cache                
                CacheHelper.Put(CacheHelper.CacheName_Conversations, cacheKeyState, hometownState);
                CacheHelper.Put(CacheHelper.CacheName_Conversations, cacheKeyCities, concatenatedCitiesInRange);
            }

            // We read from database but it was null. We put it to cache as 'null' to avoid re-read, which will lead to a null, again
            if (hometownState == "null")
            {
                hometownState = null;
            }

            if (concatenatedCitiesInRange == "null")
            {
                concatenatedCitiesInRange = null;
            }

            return hometownState;
        }

        public static string GetStateAndCitiesInRangeByLocality(SqlConnection conn, SqlTransaction trans, Locality loc, ref string concatenatedCitiesInRange)
        {
            concatenatedCitiesInRange = null;
            List<string> citiesInRange = new List<string>();
            string hometownState = MatchEngine.GetStateAndCitiesInRangeByLocality(conn, null, loc, ref citiesInRange);

            if (citiesInRange != null && citiesInRange.Count > 0)
            {
                concatenatedCitiesInRange = String.Join(",", citiesInRange.ToArray());
            }

            return hometownState;
        }

        public static string GetStateAndCitiesInRangeByLocality(SqlConnection conn, SqlTransaction trans, Locality loc, ref List<String> citiesInRange)
        {
            // check the location
            if (!IsLocationUSA(loc))
            {
                return null;
            }

            Region region = GetVerifiedRegion(conn, trans, loc);
            if (region == null)
            {
                return null;
            }

            string hometownState = region.Name;

            // check if we have county (sub-region info)
            List<City> cities = null;
            City theCity = GetVerifiedCityBySubRegion(conn, trans, loc, region, out cities);
            if (theCity != null && cities != null && cities.Count > 0)
            {
                foreach (City c in cities)
                {
                    citiesInRange.Add(c.Name);
                }

                return hometownState;
            }

            theCity = GetVerifiedCityByRegion(conn, trans, loc, region);
            if (theCity == null)
            {
                return hometownState;
            }

            // get subregion from city
            int usCountryId = 1;
            cities = City.ReadAllFromDBase(conn, trans, usCountryId, region.Id, theCity.SubRegionId);
            foreach (City c in cities)
            {
                citiesInRange.Add(c.Name);
            }

            return hometownState;
        }
    }
}
