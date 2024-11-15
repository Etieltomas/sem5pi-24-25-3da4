using System;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace Sempi5 {
    public class EmailService
    {
        public EmailService()
        {
        }

        public virtual void sendEmail(string name, string email, string subject, string message)
        {
            var _email = new MimeMessage();

            _email.From.Add(new MailboxAddress("Surgical Appointment and Resource Management", "sem5pi.isep@gmail.com")); 
            _email.To.Add(new MailboxAddress(name, email));

            _email.Subject = subject;
            _email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { 
                Text = message
            };


            using (var smtp = new SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 587, false);

                smtp.Authenticate("sem5pi.isep@gmail.com", "rqnu moxv huwn mjji "); 
                
                smtp.Send(_email);
                smtp.Disconnect(true);
            }
        }
    }
}
