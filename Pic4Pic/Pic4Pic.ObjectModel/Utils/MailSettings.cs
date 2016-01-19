using System;
using System.Text;

namespace Pic4Pic.ObjectModel
{
    public class MailSettings
    {
        public MailSettings()
        {
            this.IsEmailEnabled = true;
        }

        public bool IsEmailEnabled { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string SenderPswd { get; set; }
        public bool UseSSL { get; set; }

        public virtual bool IsValid()
        {
            if (String.IsNullOrWhiteSpace(this.SmtpHost))
            {
                return false;
            }

            if (String.IsNullOrWhiteSpace(this.SenderName))
            {
                return false;
            }

            if (String.IsNullOrWhiteSpace(this.SenderEmail))
            {
                return false;
            }

            if (!RegexUtil.IsValidEmail(this.SenderEmail))
            {
                return false;
            }

            if (String.IsNullOrWhiteSpace(this.SenderPswd))
            {
                return false;
            }

            if (this.SmtpPort <= 0)
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.AppendFormat("\t{0}:\t{1}", "IsEmailEnabled", this.IsEmailEnabled);
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "SmtpHost", this.SmtpHost);
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "SmtpPort", this.SmtpPort);
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "SenderName", this.SenderName);
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "SenderEmail", this.SenderEmail);
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "SenderPswd", this.SenderPswd);
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "UseSSL", this.UseSSL);

            return str.ToString();
        }
    }
}
