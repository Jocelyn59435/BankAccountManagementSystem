using System;
using System.IO;
using System.Net.Mail;


namespace BankAccountManagementSystem
{
    class Account
    {
        public void CreateAccount(int accountNo, string[] accountInfo, string filePath)
        {
            string[] fieldNames = { "First Name", "Last Name", "Address", "Phone", "Email" };
            using (StreamWriter outputFile = new StreamWriter(filePath))
            {
                for (int i = 0; i < accountInfo.Length; i++)
                {
                    outputFile.WriteLineAsync($"{fieldNames[i]}|{accountInfo[i]}");
                }
                outputFile.WriteLine($"AccountNo|{accountNo}");
                outputFile.WriteLine($"Balance|0");
            };
            string[] fileContentArray = File.ReadAllLines(filePath);
            string emailBody = $"Hi {accountInfo[0]}, here is your account statement: {Environment.NewLine}";
            string to = accountInfo[4];
            string from = "xinxinhuang2021@outlook.com";
            for (int i = 0; i < fileContentArray.Length; i++)
            {
                fileContentArray[i] = fileContentArray[i].Replace("|", ": ");
                emailBody += Environment.NewLine + fileContentArray[i];
            }
            try
            {
            emailBody += Environment.NewLine + Environment.NewLine + "Best Regards," + Environment.NewLine + "Simple Bank";
            MailMessage message = new MailMessage(from, to)
            {
                Subject = "Your New Account Statement",
                Body = emailBody
            };
            SmtpClient client = new SmtpClient("smtp-mail.outlook.com", 587);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("Email Account", "Password");
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Send(message);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to send mail: {e.Message}");
            }
        }

        public bool CheckAccount(string filePath)
        {
            return File.Exists(filePath);
        }

        public void DeleteAccount(string filePath)
        {
            File.Delete(filePath);
        }
    }
}
