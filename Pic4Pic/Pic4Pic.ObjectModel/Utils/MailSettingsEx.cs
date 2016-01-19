using System;
using System.Text;

namespace Pic4Pic.ObjectModel
{
    public class MailSettingsEx : MailSettings
    {
        public string EmailToList { get; set; }
        public string EmailCCList { get; set; }
        public string EmailBCCList { get; set; }

        public override bool IsValid()
        {
            if (!base.IsValid())
            {
                return false;
            }

            if (!RegexUtil.IsValidEmailList(this.EmailToList, false) &&
                !RegexUtil.IsValidEmailList(this.EmailCCList, false) &&
                !RegexUtil.IsValidEmailList(this.EmailBCCList, false))
            {
                // at least one of them must be valid
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.Append(base.ToString());
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "EmailToList", this.EmailToList);
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "EmailCCList", this.EmailCCList);
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "EmailBCCList", this.EmailBCCList);

            return str.ToString();
        }
    }
}
