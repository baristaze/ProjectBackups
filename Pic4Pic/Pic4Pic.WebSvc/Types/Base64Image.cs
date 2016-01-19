using System;
using System.Runtime.Serialization;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WebSvc
{
    [DataContract()]
    public class Base64Image
    {
        [DataMember()]
        public string ImageData { get; set; }
    }
}