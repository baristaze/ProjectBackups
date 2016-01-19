using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public class UserProfilePics
    {
        [DataMember()]
        public ImageFile FullSizeClear { get; set; }

        [DataMember()]
        public ImageFile FullSizeBlurred { get; set; }

        [DataMember()]
        public ImageFile ThumbnailClear { get; set; }

        [DataMember()]
        public ImageFile ThumbnailBlurred { get; set; }

        public static UserProfilePics From(IEnumerable<ImageFile> images)
        {
            UserProfilePics pics = new UserProfilePics();

            foreach (ImageFile img in images)
            {
                if (img.IsProfilePicture)
                {
                    if (img.IsBlurred && img.IsThumbnail)
                    {
                        pics.ThumbnailBlurred = img;       
                    }
                    else if (img.IsBlurred && !img.IsThumbnail)
                    {
                        pics.FullSizeBlurred = img;
                    }
                    else if (!img.IsBlurred && img.IsThumbnail)
                    {
                        pics.ThumbnailClear = img;
                    }
                    else if (!img.IsBlurred && !img.IsThumbnail)
                    {
                        pics.FullSizeClear = img;
                    }
                }
            }

            return pics;
        }

        public bool HasAll()
        {
            if (this.FullSizeClear == null)
            {
                return false;
            }

            if (this.FullSizeBlurred == null)
            {
                return false;
            }

            if (this.ThumbnailClear == null)
            {
                return false;
            }

            if (this.ThumbnailBlurred == null)
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

            if (this.FullSizeClear != null)
            {
                list.Add(this.FullSizeClear.Id);
            }

            if (this.FullSizeBlurred != null)
            {
                list.Add(this.FullSizeBlurred.Id);
            }

            if (this.ThumbnailClear != null)
            {
                list.Add(this.ThumbnailClear.Id);
            }

            if (this.ThumbnailBlurred != null)
            {
                list.Add(this.ThumbnailBlurred.Id);
            }

            return list;
        }
    }
}
