using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PowerIBroker.Areas.Broker.Models
{
    public class ManagelocationValidation
    {
        [DisplayName("Country")]
        [Required(ErrorMessage = "Broker's Country is required.")]
        [StringLength(100, ErrorMessage = "Country Max Length 100")]
        public string Country { get; set; }

        [DisplayName("State")]
        [Required(ErrorMessage = "Broker's State is required.")]
        [StringLength(100, ErrorMessage = "State Max Length 100")]
        public string State { get; set; }

        [DisplayName("Location")]
        [Required(ErrorMessage = "Location required.")]
        [StringLength(100, ErrorMessage = "Location Max Length 100")]
        public string Location { get; set; }

        [DisplayName("Zip Code")]
        //[StringLength(8, ErrorMessage = "Zip Code Max Length 8")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please enter valid Zip Code")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Please enter valid Zip Code")]
        public string ZipCode { get; set; }


        public Nullable<long> CountryID { get; set; }

        public long ID { get; set; }

        public Nullable<long> BrokerId { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public string StateID { get; set; }    
      

        public Nullable<System.DateTime> CreatedOn { get; set; }

   
    }
}

