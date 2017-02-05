using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace WindowsService1
{
    class Updates
    {
        public Updates(string URL)
        {
            var file = URL.Split('/');
            file = (file[file.Length - 1]).Split('.');
            var filename = file[0];

            System.Net.WebClient wc = new System.Net.WebClient();
            byte[] raw = wc.DownloadData(URL);

            string newData = System.Text.Encoding.UTF8.GetString(raw);

            if (File.Exists(filename))
            {
                var oldData = File.ReadAllText(filename);
                PageIsDifferent = IsDifferent(newData, oldData);
            }
            else
            {
                using (var fs = File.Open(filename, FileMode.CreateNew))
                {
                    try
                    {
                        fs.Write(raw, 0, raw.Length);
                    }
                    catch(Exception ex)
                    {
                        System.Diagnostics.Debug.Assert(false, ex.Message);
                    }
                }
            }
        }

        private bool IsDifferent(string oldData, string newData)
        {
            return string.Compare(oldData, newData) != 0;
        }

        public void SendMail(string credFilePath)
        {
            var fs = File.ReadAllLines(credFilePath);
            if (fs.Length != 1)
            {
                System.Diagnostics.Debug.Assert(false, "Credential file contains more lines than expected.");
            }

            var credData = fs[0].Split(' ');
            var senderEmail = credData[0];
            var pwd = credData[1];
            var receiverEmail = credData[2];

            MailMessage mail = new MailMessage(senderEmail, receiverEmail);
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "smtp.live.com";
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(senderEmail, pwd);// New System.Net.NetworkCredential()
            mail.Subject = "There are some changes in PNP webpage.";
            mail.Body = "Please follow below link to review. <br> http://www.ontarioimmigration.ca/en/pnp/OI_PNPNEW.html";

            client.Send(mail);
        }

        public bool PageIsDifferent = false;
    }
}
