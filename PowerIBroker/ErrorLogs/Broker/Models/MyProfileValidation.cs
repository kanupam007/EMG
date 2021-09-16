using System;

namespace PowerIBroker.Areas.Broker.Models
{
    public class MyProfileValidation
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BrokerName { get; set; }  
        public string Email { get; set; }      
        public string ContactNo { get; set; }        
        public string AboutProfile { get; set; }

        //-------------------------------------------------
      
        public int CardTypeValue { get; set; }
        public string CardHolderName { get; set; }       
        public string CardNumber { get; set; }      
        public string CardCVVNo { get; set; }       
        public int CardExpMonth { get; set; }
        public int CardExpYear { get; set; }
        public string ExpireOn { get; set; }
        //-------------------------------------------------------------

        public long ID { get; set; }
        public Nullable<long> MasterID { get; set; }
        public string Zipcode { get; set; }
        public string LogoImage { get; set; }
        public string BrokerImage { get; set; }
    
    }
}