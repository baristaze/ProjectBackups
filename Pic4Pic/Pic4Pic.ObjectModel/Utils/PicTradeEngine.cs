using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public class PicTradeEngine
    {
        public static PicForPic RequestPic4Pic(
            SqlConnection conn, 
            SqlTransaction trans, 
            Guid userId, 
            Guid userIdToInteract, 
            Guid pictureIdToExchange, 
            int duplicateProtectionPeriodAsMinutes,
            ref Familiarity currentFamiliarity)
        {
            // check input
            if (userId == Guid.Empty)
            {
                throw new Pic4PicException("Invalid User ID");
            }

            // check input
            if (userIdToInteract == Guid.Empty)
            {
                throw new Pic4PicException("Invalid User ID to interact");
            }

            // more check
            if (userId == userIdToInteract)
            {
                throw new Pic4PicException("Something is wrong (internal error)!");
            }

            // check input
            if (pictureIdToExchange == Guid.Empty)
            {
                throw new Pic4PicException("Invalid Picture ID to exchange");
            }

            // get matching user...
            List<Familiarity> familiarities = new List<Familiarity>();
            List<User> matchingUsers = User.ReadAllFromDBaseWithFamiliarity(
                conn, trans, userIdToInteract.ToString(), userId, ref familiarities);

            // check user
            if(matchingUsers.Count <= 0)
            {
                throw new Pic4PicException("Referenced user couldn't be found");
            }

            // get matching user
            User matchingUser = matchingUsers[0];
            Familiarity familiarity = familiarities[0];

            // get pictures of current user
            bool imageFound = false;
            List<ImageFile> picturesOfCurrentUser = ImageFile.ReadAllFromDBaseByUserId(conn, trans, userId);
            foreach (ImageFile img in picturesOfCurrentUser) 
            {
                if (img.GroupingId == pictureIdToExchange) 
                {
                    imageFound = true;
                    break;
                }
            }
            if (!imageFound)
            {
                throw new Pic4PicException("Referenced picture couldn't be found");
            }

            // prepare pic4pic request
            PicForPic pic4picRequest = new PicForPic();
            pic4picRequest.UserId1 = userId;
            pic4picRequest.UserId2 = userIdToInteract;
            pic4picRequest.PicId1 = pictureIdToExchange;

            // check profile pictures
            UserProfilePics currentUserProfilePictures = UserProfilePics.From(picturesOfCurrentUser);
            if (!currentUserProfilePictures.HasAll())
            {
                throw new Pic4PicException("Something is wrong with your profile pictures");
            }

            // act based on scenario
            if(familiarity == Familiarity.Stranger)
            {   
                // check profile picture
                if (currentUserProfilePictures.ThumbnailBlurred.GroupingId != pictureIdToExchange)
                {
                    throw new Pic4PicException("First picture to exchange should have been your profile picture. (Internal error)");
                }
            }
            else if(familiarity == Familiarity.Familiar)
            {
                // check profile picture
                if (currentUserProfilePictures.ThumbnailBlurred.GroupingId == pictureIdToExchange)
                {
                    throw new Pic4PicException("Your profile picture cannot be used in pic4pic requests except the first time. (Internal error)");
                }
            }
            else
            {
                throw new Pic4PicException("Internal Error: Unknown familiarity!");
            }

            // check to see if this is a duplicate request
            DateTime cutOffTime = DateTime.UtcNow.AddMinutes(-1 * duplicateProtectionPeriodAsMinutes);
            List<PicForPic> previousRequests = PicForPic.ReadAllFromDBase(conn, trans, userId, userIdToInteract, 300);
            foreach (PicForPic pre in previousRequests)
            {
                if(pre.RequestTimeUTC < cutOffTime)
                {
                    // we don't care requests older than certain days
                    // continue;
                    break; // not continue but break because previousRequests are sorted by [RequestTimeUTC DESC]
                }

                if (pic4picRequest.IsDuplicate(pre, true))
                {
                    if (!pre.IsAccepted())
                    {
                        throw new Pic4PicException("You've already sent a pic4pic request to this user recently!");
                    }
                    else
                    {
                        throw new Pic4PicException("This user has already accepted your recent pic4pic request!");
                    }
                }
                else if (pic4picRequest.IsCrossMatch(pre, true))
                {
                    if (!pre.IsAccepted())
                    {
                        throw new Pic4PicException("This user has sent you a similar request recently. Check your notifications!");
                    }
                    else
                    {
                        throw new Pic4PicException("Oops! You've already accepted a similar request from this user recently.");
                    }
                }                
            }

            // create pic4pic request
            Guid pic4picRequestId = PicForPic.CreateRequestOnDBase(conn, trans, pic4picRequest, false);

            // return 
            currentFamiliarity = familiarity;
            pic4picRequest.Id = pic4picRequestId;
            return pic4picRequest;
        }


        public static PicForPic AcceptPic4PicRequest(
           SqlConnection conn,
           SqlTransaction trans,
           Guid currentUserId,
           Guid pic4picRequestId,
           Guid pictureIdToExchange)
        {
            // check input
            if (currentUserId == Guid.Empty)
            {
                throw new Pic4PicException("Invalid User ID");
            }

            // check input
            if (pic4picRequestId == Guid.Empty)
            {
                throw new Pic4PicException("Invalid Pic4Pic Request ID");
            }
            
            // check input
            if (pictureIdToExchange == Guid.Empty)
            {
                throw new Pic4PicException("Invalid Picture ID to exchange");
            }

            PicForPic pic4pic = PicForPic.ReadFromDBase(conn, trans, pic4picRequestId);
            if (pic4pic == null)
            {
                throw new Pic4PicException("The specified pic4pic request couldn't be found.");
            }

            if (pic4pic.UserId2 != currentUserId)
            {
                throw new Pic4PicException("The specified pic4pic request couldn't be found (2).");
            }

            if (pic4pic.IsAccepted())
            {
                if (pic4pic.HasBothPictures())
                {
                    if (pictureIdToExchange == pic4pic.PicId2) // sending the request twice... let it go...
                    {
                        return pic4pic;
                    }
                    else
                    {
                        throw new Pic4PicException("You have already accepted this request before!"); // with another picture though!
                    }
                }
                else
                {
                    throw new Pic4PicException("Something is wrong (internal error)!");
                }
            }

            if (pic4pic.PicId2 != Guid.Empty)
            {
                if (pic4pic.PicId2 != pictureIdToExchange)
                {
                    throw new Pic4PicException("You've already accepted this request before!");
                }
                else
                {
                    // why accept time hasn't been set...
                    throw new Pic4PicException("Something is wrong (internal error 2)!");
                }
            }
            else
            {
                // check to see if this picture belongs to current user
                ImageFile imageFile = ImageFile.ReadFromDBase(conn, trans, pictureIdToExchange, currentUserId);
                if (imageFile == null)
                {
                    throw new Pic4PicException("Invalid picture Id to exchange (2).");
                }
                else
                {
                    // set the picture
                    pic4pic.PicId2 = pictureIdToExchange;
                }
            }

            // update pic4pic
            pic4pic.AcceptTimeUTC = DateTime.UtcNow;

            // save on database
            int affectedRows = PicForPic.AcceptRequestOnDBase(conn, trans, pic4pic, true);

            // return
            return pic4pic;
        }
    }
}
