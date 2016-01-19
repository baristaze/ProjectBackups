using System;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Pic4Pic.ObjectModel;
using System.Text;

namespace Pic4Pic.WebSvc
{
    [DataContract()]
    public class ClientLogRequest : BaseRequest
    {
        [DataMember()]
        public List<LogBag> Logs 
        { 
            get 
            { 
                return this.logs; 
            }
            set
            {
                if (value != null)
                {
                    this.logs = value;
                }
                else
                {
                    this.logs = new List<LogBag>();
                }
            }
        }
        private List<LogBag> logs = new List<LogBag>();

        public override void Validate()
        {
            // base.Validate();

            if (this.logs.Count <= 0)
            {
                throw new Pic4PicArgumentException("Logs property of ClientLogRequest is empty", "Logs");
            }

            foreach (LogBag bag in this.logs)
            {
                bag.Validate();
            }
        }
    }
}