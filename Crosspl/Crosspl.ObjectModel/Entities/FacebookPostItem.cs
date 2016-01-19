using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace Crosspl.ObjectModel
{
    public class FacebookPostItem
    {
        /// <summary>
        /// e.g. http(s)://mydomain.com/apple
        /// </summary>
        public string ItemLink { get; set; }

        /// <summary>
        /// e.g. http(s)://imagedepot.com/image/333
        /// </summary>
        public string ItemPicture { get; set; }

        /// <summary>
        /// e.g. Matrix Reloaded
        /// </summary>
        public string ItemCaption { get; set; }

        /// <summary>
        /// blah blah blah... a long description
        /// </summary>
        public string ItemDescription { get; set; }

        /// <summary>
        /// e.g. I watched this movie today and enchanted!
        /// </summary>
        public string PersonalMessage { get; set; }

        /// <summary>
        /// This could be a person name (Baris Taze) or an app name; e.g. Crosspl
        /// </summary>
        public string ActorName { get; set; }

        /// <summary>
        /// Creates a dynamic (typeless) version of this object
        /// </summary>
        /// <returns></returns>
        internal dynamic GetAsDynamicObject()
        {
            dynamic messagePost = new ExpandoObject();
            
            if (!String.IsNullOrWhiteSpace(this.ItemPicture))
            {
                messagePost.picture = this.ItemPicture;
            }

            if (!String.IsNullOrWhiteSpace(this.ItemLink))
            { 
                messagePost.link = this.ItemLink;
            }

            if (!String.IsNullOrWhiteSpace(this.ItemCaption))
            { 
                messagePost.caption = this.ItemCaption;
            }

            if (!String.IsNullOrWhiteSpace(this.ItemDescription))
            { 
                messagePost.description = this.ItemDescription;
            }

            if (!String.IsNullOrWhiteSpace(this.PersonalMessage))
            { 
                messagePost.message = this.PersonalMessage;
            }

            if (!String.IsNullOrWhiteSpace(this.ActorName))
            { 
                messagePost.name = this.ActorName;
            }
            
            return messagePost;
        }
    }
}
