using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public class PicturePair
    {
        [DataMember()]
        public ImageFile FullSize { get; set; }

        [DataMember()]
        public ImageFile Thumbnail { get; set; }

        public static PicturePair GetProfilePicturePair(IEnumerable<ImageFile> images, Familiarity familiarity)
        {
            PicturePair pics = new PicturePair();

            foreach (ImageFile img in images)
            {
                if (img.IsProfilePicture)
                {
                    if (img.IsThumbnail)
                    {
                        if (img.IsBlurred && (familiarity == Familiarity.Stranger))
                        {
                            pics.Thumbnail = img;
                        }
                        else if (!img.IsBlurred && (familiarity == Familiarity.Familiar))
                        {
                            pics.Thumbnail = img;
                        }
                    }
                    else 
                    {
                        if (img.IsBlurred && (familiarity == Familiarity.Stranger))
                        {
                            pics.FullSize = img;
                        }
                        else if (!img.IsBlurred && (familiarity == Familiarity.Familiar))
                        {
                            pics.FullSize = img;
                        }
                    }                    
                }
            }

            return pics;
        }

        public static Dictionary<Guid, List<ImageFile>> GroupImagesByGroupId(IEnumerable<ImageFile> images)
        {
            Dictionary<Guid, List<ImageFile>> groupedPics = new Dictionary<Guid, List<ImageFile>>();
            foreach (ImageFile img in images)
            {
                if (!groupedPics.ContainsKey(img.GroupingId))
                {
                    groupedPics.Add(img.GroupingId, new List<ImageFile>());
                }

                groupedPics[img.GroupingId].Add(img);
            }

            return groupedPics;
        }

        // this method always return clear images; i.e. non-pixelized images
        public static List<PicturePair> GetNonProfilePicturePairs(IEnumerable<ImageFile> images) 
        {
            Dictionary<Guid, List<ImageFile>> groupedPics = GroupImagesByGroupId(images);

            List<PicturePair> result = new List<PicturePair>();
            foreach (Guid groupingId in groupedPics.Keys)
            {
                List<ImageFile> group = groupedPics[groupingId];

                PicturePair pair = new PicturePair();
                foreach (ImageFile img in group)
                {
                    if (!img.IsProfilePicture)
                    {
                        if (img.IsThumbnail)
                        {
                            if (!img.IsBlurred) 
                            {
                                pair.Thumbnail = img;
                            }                            
                        }
                        else
                        {
                            if (!img.IsBlurred)
                            {
                                pair.FullSize = img;
                            }
                        }
                    }
                }

                if (pair.HasAll())
                {
                    result.Add(pair);
                }
            }

            return result;
        }

        public static List<PicturePair> GetNonProfilePicturePairs(IEnumerable<ImageFile> images, List<PicForPic> pic4pics)
        {
            Dictionary<Guid, List<ImageFile>> groupedPics = GroupImagesByGroupId(images);
            Dictionary<Guid, List<PicForPic>> pic4picsByImgGroupId = PicForPic.GroupByImageGroupId(pic4pics);

            List<PicturePair> result = new List<PicturePair>();
            foreach (Guid groupingId in groupedPics.Keys)
            {
                List<ImageFile> group = groupedPics[groupingId];

                bool isAccepted = false;
                if (pic4picsByImgGroupId.ContainsKey(groupingId))
                {
                    List<PicForPic> pic4picsOfThisGroup = pic4picsByImgGroupId[groupingId];
                    foreach (PicForPic p4p in pic4picsOfThisGroup)
                    {
                        if (p4p.IsAccepted())
                        {
                            isAccepted = true;
                            break;
                        }
                    }
                }

                PicturePair pair = new PicturePair();
                foreach (ImageFile img in group)
                {
                    if (!img.IsProfilePicture)
                    {
                        if (img.IsThumbnail)
                        {
                            if (img.IsBlurred && !isAccepted)
                            {
                                pair.Thumbnail = img;
                            }
                            else if (!img.IsBlurred && isAccepted)
                            {
                                pair.Thumbnail = img;
                            }
                        }
                        else
                        {
                            if (img.IsBlurred && !isAccepted)
                            {
                                pair.FullSize = img;
                            }
                            else if (!img.IsBlurred && isAccepted)
                            {
                                pair.FullSize = img;
                            }
                        }
                    }
                }

                if (pair.HasAll())
                {
                    result.Add(pair);
                }
            }

            return result;
        }

        public bool HasAll()
        {
            if (this.FullSize == null)
            {
                return false;
            }

            if (this.Thumbnail == null)
            {
                return false;
            }

            return true;
        }

        public bool HasAll(List<Guid> imageIds)
        {
            List<Guid> list = this.GetImageIds();
            if (list.Count != imageIds.Count)
            {
                return false;
            }

            foreach (Guid id in imageIds) {
                if (!list.Contains(id)) {
                    return false;
                }
            }

            return true;
        }

        public bool HasAny(List<Guid> imageIds)
        {
            List<Guid> list = this.GetImageIds();
            foreach (Guid id in imageIds)
            {
                if (list.Contains(id))
                {
                    return true;
                }
            }

            return false;
        }

        public List<Guid> GetImageIds()
        {
            List<Guid> list =new List<Guid>();

            if (this.FullSize != null)
            {
                list.Add(this.FullSize.Id);
            }
            
            if (this.Thumbnail != null)
            {
                list.Add(this.Thumbnail.Id);
            }

            return list;
        }
    }
}
