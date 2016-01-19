using System;
using System.Runtime.Serialization;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WebSvc
{
    [DataContract()]
    public class VerifyBioRequest : FacebookRequest
    {
        [DataMember()]
        public string UserFields { get; set; }

        internal string[] UserFieldsAsArray 
        {
            get 
            {
                if(String.IsNullOrWhiteSpace(this.UserFields))
                {
                    return new string[0];
                }
                else
                {
                    return this.UserFields.Split(
                        new char[] { ',', ';', ' ' }, 
                        StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }
    }
}
