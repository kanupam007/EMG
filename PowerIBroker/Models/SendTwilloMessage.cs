using PowerIBrokerBusinessLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Twilio;


namespace PowerIBroker.Models
{
    public class SendTwilloMessage
    {
        public static int SendmessageTwillo(string To, string Msg)
        {
            // get credential from utility
            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            string PageURL = (string)settingsReader.GetValue("PageURL", typeof(String));
            string SMSNotificationOnOff = (string)settingsReader.GetValue("SMSNotificationOnOff", typeof(String));
            CommonMasters cmm = new CommonMasters();
            var SystemConfigurationAdminData = cmm.GetSystemConfigurationAdmin(0);
            string AccountSid = SystemConfigurationAdminData.Where(s => s.PropertyName == "APIAccountIDTwillo").FirstOrDefault().Value;// System.Configuration.ConfigurationManager.AppSettings["APIAccountID"].ToString();
            string AuthToken = SystemConfigurationAdminData.Where(s => s.PropertyName == "APIAccountTokenTwillo").FirstOrDefault().Value;//System.Configuration.ConfigurationManager.AppSettings["APIAccountToken"].ToString();
            string smsMobileNo = SystemConfigurationAdminData.Where(s => s.PropertyName == "SmsMobileTwillo").FirstOrDefault().Value; //System.Configuration.ConfigurationManager.AppSettings["SmsMobile"].ToString();
            string Countrycode = SystemConfigurationAdminData.Where(s => s.PropertyName == "CountrycodeTwillo").FirstOrDefault().Value;//System.Configuration.ConfigurationManager.AppSettings["Countrycode"].ToString();
            bool SMSNotification = Convert.ToBoolean(SystemConfigurationAdminData.Where(s => s.PropertyName == "SMSNotification").FirstOrDefault().IsActive);
           To = Countrycode + To;

            string Result = "";
            int i = 0;
            try
            {
                Result = "success";
                ////Uncomment Below Code to Send SMS
                var twilio = new TwilioRestClient(AccountSid, AuthToken);
                if (twilio != null)
                {
                    Message message1 = new Message();
                    if (SMSNotification)
                    {
                        if(!string.IsNullOrEmpty(To))
                        {
                            message1 = twilio.SendMessage(smsMobileNo, To, Msg);      // +15005550006 magic number , +1 626-264-8205
                        }
                       
                    }
              

                    if (message1.RestException != null)
                    {

                        Result = message1.RestException.Message;
                    }
                    else
                    {

                        Result = "Success";
                        i = 1;
                    }
                }
                else
                {

                    Result = "Not Success";
                    i = 0;
                }
            }
            catch (Exception ex)
            {

                Result = ex.Message;
            }
            return i;

        }
    }
}