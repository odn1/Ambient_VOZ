using AmbientService.src.models;
using AmbientService.src.utils;
using System;
using System.Net.NetworkInformation;
using System.Text;

namespace AmbientService.src.controllers
{
    public class EmailController
    {
        #region Singlton
        private static EmailController instatnse;

        public static EmailController GetInstanse()
        {
            return instatnse is null ? instatnse = new EmailController() : instatnse;
        }
        #endregion

        private EmailController()
        {
            EventCollection.OutBeyond += SendOutBeyond;
            EventCollection.InBeyond += SendInBeyond;
            EventCollection.LowBattery += SendLowBattery;
            EventCollection.NotContact += SendNotContact;
            EventCollection.OnContact += SendOnContact;
        }

        private void SendNotContact(Sensor data)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(data.Measurement.Time.ToString("G"));
            Console.WriteLine(data.Measurement.Time);
            builder.AppendLine("Sensor " + data.Name);
            builder.AppendLine("No Connect");
            SendEmail(builder.ToString(), "Sensor " + data.Name);
        }

        private void SendOnContact(Sensor data)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(data.Measurement.Time.ToString("G"));
            builder.AppendLine("Sensor " + data.Name);
            builder.AppendLine("Yes Connect");
            SendEmail(builder.ToString(), "Sensor " + data.Name);
        }

        private void SendOutBeyond(Sensor data)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(data.Measurement.Time.ToString("G"));
            builder.AppendLine("Sensor " + data.Name);
            builder.AppendLine(data.GetBoundString("Out Limit"));

            SendEmail(builder.ToString(), "Sensor " + data.Name);
        }

        private void SendInBeyond(Sensor data)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(data.Measurement.Time.ToString("G"));
            builder.AppendLine("Sensor " + data.Name);
            builder.AppendLine(data.GetBoundString("In Limit"));

            SendEmail(builder.ToString(), "Sensor " + data.Name);
        }

        private void SendLowBattery(Sensor data)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(data.Measurement.Time.ToString("G"));
            builder.AppendLine("Sensor " + data.Name);
            builder.AppendLine("Low battery level");
            builder.AppendLine("Ubat min = 3.5,  Ubat = " + data.Measurement.BatteryLevel);

            SendEmail(builder.ToString(), "Sensor " + data.Name);
        }

        public void SendEmail(string text, string tytle)
        {
            try
            {
                string yourEmail = IniReader.Read("email", "Email");
                CDO.Message message = new CDO.Message();
                CDO.IConfiguration configuration = message.Configuration;
                ADODB.Fields fields = configuration.Fields;

                Console.WriteLine(String.Format("Configuring CDO settings..."));

                ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
                //  field.Value = "smtp.yandex.ru";
                field.Value = "mail.belkarolin.com";

                field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
                field.Value = 2525;
                // field.Value = 465;

                field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
                field.Value = CDO.CdoSendUsing.cdoSendUsingPort;

                field = fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"];
                // field.Value = CDO.CdoProtocolsAuthentication.cdoBasic;
                field.Value = CDO.CdoProtocolsAuthentication.cdoBasic;

                field = fields["http://schemas.microsoft.com/cdo/configuration/sendusername"];
                field.Value = yourEmail;

                field = fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"];

                //  field.Value = "mrhdytxjrrxtduxh";
                field.Value = IniReader.Read("password", "Email");

                field = fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"];
                //field.Value = "true"; //защита соед
                field.Value = "false";

                fields.Update();

                Console.WriteLine(String.Format("Building CDO Message..."));

                message.From = yourEmail;
                message.To = yourEmail;
                message.Subject = tytle;//"Info message";
                message.TextBody =  text;

                Console.WriteLine(String.Format("Attempting to connect to remote server..."));
                Console.WriteLine(text);
                // Send message.
                message.Send();


                Console.WriteLine("Message sent.");
            } 
            catch (Exception ex )
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
