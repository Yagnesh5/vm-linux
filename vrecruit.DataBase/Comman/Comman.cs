using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using vrecruit.DataBase.EntityDataModel;
using vrecruit.DataBase.ViewModel;

namespace vrecruit.DataBase.Comman
{
    public class Comman
    {
       // public bool SendEmail(string Subject, string email, string passwordlink, string body)
        public bool SendEmail(EmailViewModels model)
        {
            try
            {
                SmtpClient mailServer = new SmtpClient(model.Smtp, model.Port);
                mailServer.EnableSsl = true;
                mailServer.UseDefaultCredentials = false;
                mailServer.Credentials = new System.Net.NetworkCredential(model.Smtpemail, model.Smtppassword);
                //string from = UserEmail.ToString();
                //string to = CandidateEmail.ToString();
                string from = model.Smtpemail;
                string to = model.to;
                MailMessage msg = new MailMessage(from, to);
                if (model.email != null && model.passwordlink != null)
                {
                    msg.Subject = "Change password";
                    msg.Body = "Reset your password from this link : " + model.passwordlink;
                }
                else
                {
                    msg.Subject = model.Subject;
                    msg.IsBodyHtml = true;
                    msg.Body= model.body;
                    //msg.Attachments.Add(new Attachment("D:\\myfile.txt"));
                }
                mailServer.Send(msg);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }

}
