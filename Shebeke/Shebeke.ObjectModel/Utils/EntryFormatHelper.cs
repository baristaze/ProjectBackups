using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Shebeke.ObjectModel
{
    public class EntryFormatHelper
    {
        public static void FormatEntriesAndAttachImages(
            SqlConnection conn, 
            SqlTransaction trans, 
            UserAuthInfo user, 
            List<Entry> entries, 
            string appPath,
            bool includeAuthorInfo)
        {
            // get author info
            List<FacebookUser> authors = new List<FacebookUser>();
            if (includeAuthorInfo)
            {
                // it checks the cache first
                string userIDs = ShebekeUtils.ConcatenateIDs(entries, (e) => { return e.CreatedBy; });
                authors = FacebookUser.ReadAllByID(conn, trans, userIDs);
            }

            // get image source of the entries
            string entryIDs = ShebekeUtils.ConcatenateIDs(entries);
            List<ImageFile> images = ImageFile.ReadFromDBaseByAssetIDs(conn, trans, AssetType.Entry, entryIDs);
            foreach (Entry e in entries)
            {
                // set author info
                if (includeAuthorInfo)
                {
                    e.AuthorInfo = new AuthorInfo();
                    e.AuthorInfo.Id = e.CreatedBy;
                    FacebookUser author = ShebekeUtils.Search(authors, e.CreatedBy);
                    if (author != null)
                    {
                        e.AuthorInfo.FirstName = author.FirstName;
                        e.AuthorInfo.LastName = author.LastName;
                        if (String.IsNullOrWhiteSpace(author.PhotoUrl))
                        {
                            e.AuthorInfo.PhotoUrl = "https://graph.facebook.com/" + author.FacebookId.ToString() + "/picture?type=small";
                        }
                        else
                        {
                            e.AuthorInfo.PhotoUrl = author.PhotoUrl;
                        }
                    }
                    else
                    {
                        e.AuthorInfo.FirstName = "Misafir";
                        e.AuthorInfo.PhotoUrl = appPath + "/Images/user.png";
                    }
                }

                // html-encode the content
                List<ImageFile> filteredImages = FilterImages(images, e.Id);
                IEntryFormatter formatter = EntryFormatterAbstractFactory.CreateForCurrent(appPath);
                e.ContentAsEncodedHtml = formatter.GetEncodedHtml(e.Content, filteredImages, null);
                e.CanDelete = ((int)user.UserType > (int)UserType.Regular) || (user.UserType == UserType.Regular && user.UserId == e.CreatedBy);
                e.CanEdit = ((int)user.UserType > (int)UserType.Regular) || (user.UserType == UserType.Regular && user.UserId == e.CreatedBy);
            }
        }

        public static List<ImageFile> FilterImages(List<ImageFile> images, long assetId)
        {
            List<ImageFile> temp = new List<ImageFile>();
            foreach (ImageFile i in images)
            {
                if (i.AssetId == assetId)
                {
                    temp.Add(i);
                }
            }

            return temp;
        }
    }
}
