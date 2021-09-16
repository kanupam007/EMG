using System;
using System.IO;
using System.Web;

namespace PowerIBroker.Models
{
    public class ErrorLog
    {
        #region Function for mainataining log error save in database
        public void CustomErrorLog(string ErrorMessage, string ErrorPage, string Errorfunction, string ProcessType)
        {
            string filename = HttpContext.Current.Server.MapPath("/Content/ErrorLog/") + "Log_" + String.Format("{0:y}", System.DateTime.Now) + ".txt";
            string filepath = filename;

            if (File.Exists(filepath))
            {
                using (StreamWriter writer = new StreamWriter(filepath, true))
                {
                    writer.WriteLine("-------------------START-------------" + System.DateTime.Now + "\r\n");
                    writer.WriteLine("Controller Name : " + ErrorPage);
                    writer.WriteLine("Function in which error found : " + Errorfunction);
                    writer.WriteLine("Process Type : " + ProcessType);
                    writer.WriteLine("Error Message : " + ErrorMessage);
                    writer.WriteLine("-------------------END-------------" + System.DateTime.Now + "\r\n");
                }
            }
            else
            {
                StreamWriter writer = File.CreateText(filepath);
                writer.WriteLine("-------------------START-------------" + System.DateTime.Now + "\r\n");
                writer.WriteLine("Controller Name : " + ErrorPage);
                writer.WriteLine("Function in which error found : " + Errorfunction);
                writer.WriteLine("Process Type : " + ProcessType);
                writer.WriteLine("Error Message : " + ErrorMessage);
                writer.WriteLine("-------------------END-------------" + System.DateTime.Now + "\r\n");
                writer.Close();
            }

        }
        #endregion


        #region Function for mainataining log error save in database
        public void CustomErrorMessage(string ErrorMessage)
        {
            string filename = HttpContext.Current.Server.MapPath("/Content/ErrorLog/") + "Error_Message.txt";
            string filepath = filename;
            if (File.Exists(filepath))
            {
                using (StreamWriter writer = new StreamWriter(filepath, true))
                {
                    writer.WriteLine("Error Message : " + ErrorMessage);
                }
            }
            else
            {
                StreamWriter writer = File.CreateText(filepath);
               
                writer.WriteLine("Error Message : " + ErrorMessage);
                writer.Close();
            }

        }
        #endregion
    
    }
}