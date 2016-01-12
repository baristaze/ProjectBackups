using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace YummyZone.ObjectModel
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

            if (!MailSender.IsValidEmail(this.SenderEmail))
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

            if (!MailSender.IsValidEmailList(this.EmailToList, false) &&
                !MailSender.IsValidEmailList(this.EmailCCList, false) &&
                !MailSender.IsValidEmailList(this.EmailBCCList, false))
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

    public class MailSender
    {
        public MailSettings Settings { get; private set; }

        public MailSender(MailSettings settings)
        {
            this.Settings = settings;
        }

        public bool TrySend(string subject, string htmlContent, string toList)
        {
            return this.TrySend(subject, htmlContent, toList, null);
        }

        public bool TrySend(string subject, string htmlContent, string toList, string ccList)
        {
            return this.TrySend(subject, htmlContent, toList, ccList, null);
        }

        public bool TrySend(string subject, string htmlContent, string toList, string ccList, string bccList)
        {
            try
            {
                return this.Send(subject, htmlContent, toList, ccList, bccList);
            }
            catch
            {
                return false;
            }
        }

        public bool Send(string subject, string htmlContent, string toList, string ccList, string bccList)
        {
            if (this.Settings == null)
            {
                return false;
            }

            if (!this.Settings.IsValid())
            {
                return false;
            }

            if (String.IsNullOrWhiteSpace(subject))
            {
                return false;
            }

            if (String.IsNullOrWhiteSpace(htmlContent))
            {
                return false;
            }

            SmtpClient smtpClient = new SmtpClient(this.Settings.SmtpHost, this.Settings.SmtpPort);
            smtpClient.Credentials = new System.Net.NetworkCredential(this.Settings.SenderEmail, this.Settings.SenderPswd);
            smtpClient.EnableSsl = this.Settings.UseSSL;

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(this.Settings.SenderEmail, this.Settings.SenderName, System.Text.Encoding.UTF8);

            bool atLeastOneReceiver = false;

            if (!String.IsNullOrWhiteSpace(toList))
            {
                string[] tos = toList.Split(';');
                foreach (string to in tos)
                {
                    if (MailSender.IsValidEmail(to))
                    {
                        msg.To.Add(new MailAddress(to));
                        atLeastOneReceiver = true;
                    }
                }
            }

            if (!String.IsNullOrWhiteSpace(ccList))
            {
                string[] ccs = ccList.Split(';');
                foreach (string cc in ccs)
                {
                    if (MailSender.IsValidEmail(cc))
                    {
                        msg.CC.Add(new MailAddress(cc));
                        atLeastOneReceiver = true;
                    }
                }
            }

            if (!String.IsNullOrWhiteSpace(bccList))
            {
                string[] bccs = bccList.Split(';');
                foreach (string bcc in bccs)
                {
                    if (MailSender.IsValidEmail(bcc))
                    {
                        msg.Bcc.Add(new MailAddress(bcc));
                        atLeastOneReceiver = true;
                    }
                }
            }

            if (!atLeastOneReceiver)
            {
                return false;
            }

            msg.Subject = subject;
            msg.IsBodyHtml = true;
            msg.Body = htmlContent;

            smtpClient.Send(msg);
            return true;
        }

        public static bool IsValidEmail(string email)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(email,
                   @"^(?("")(""[^""]+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
        }

        public static bool IsValidEmailList(string emails, bool isEmptyOK)
        {
            string[] tokens = emails.Split(';');
            if (tokens.Length == 0)
            {
                return isEmptyOK;
            }

            foreach (string token in tokens)
            {
                if (!IsValidEmail(token))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
