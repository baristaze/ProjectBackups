using System;
using System.Diagnostics;
using System.Net.Mail;

namespace Pic4Pic.ObjectModel
{
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
                return this.Send(subject, htmlContent, toList, ccList, bccList, null);
            }
            catch(Exception ex)
            {
                Logger.bag().Add(ex).LogError("Error when sending email.");
                return false;
            }
        }

        public bool TrySend(string subject, string htmlContent, string toList, string ccList, string bccList, string replyToList)
        {
            try
            {
                return this.Send(subject, htmlContent, toList, ccList, bccList, replyToList);
            }
            catch (Exception ex)
            {
                Logger.bag().Add(ex).LogError("Error when sending email.");
                return false;
            }
        }

        public bool Send(string subject, string htmlContent, string toList, string ccList, string bccList, string replyToList)
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
                    if (RegexUtil.IsValidEmail(to))
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
                    if (RegexUtil.IsValidEmail(cc))
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
                    if (RegexUtil.IsValidEmail(bcc))
                    {
                        msg.Bcc.Add(new MailAddress(bcc));
                        atLeastOneReceiver = true;
                    }
                }
            }

            if (!String.IsNullOrWhiteSpace(replyToList))
            {
                string[] replies = replyToList.Split(';');
                foreach (string replyTo in replies)
                {
                    if (RegexUtil.IsValidEmail(replyTo))
                    {
                        msg.ReplyToList.Add(new MailAddress(replyTo));
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
    }
}
