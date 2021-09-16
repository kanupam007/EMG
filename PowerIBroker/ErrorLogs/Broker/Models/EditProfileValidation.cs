
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PowerIBroker.Areas.Broker.Models
{
    public class EditProfileValidation
    {

        [DisplayName("Broker Name")]
        [Required(ErrorMessage = "The Broker's Name required.")]
        public string FirstName { get; set; }

        [DisplayName("Broker Email")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "The Broker's email is required.")]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter valid email")]
        [StringLength(50, ErrorMessage = "Email Max Length 50 ")]
        public string Email { get; set; }

        [DisplayName("Contact Number")]
        [Required(ErrorMessage = "The Broker's contact is required.")]
        // [StringLength(13, ErrorMessage = "Contact Max Length 10")]
        //[Range(0, long.MaxValue, ErrorMessage = "Please enter valid contact")]
        [StringLength(int.MaxValue, MinimumLength = 10, ErrorMessage = "Min Length is 10")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter valid contact")]
        public string ContactNo { get; set; }

        [DisplayName("Profile")]
        [StringLength(1000, ErrorMessage = "Profile Max Length 1000")]
        public string AboutProfile { get; set; }

        //-------------------------------------------------
        [DisplayName("Card Type")]
        [Required(ErrorMessage = "Select Card Type.")]
        public int CardTypeValue { get; set; }

        [DisplayName("Card Holder Name")]
        [Required(ErrorMessage = "The Card Holder Name required.")]
        [StringLength(50, ErrorMessage = "Card Holder Name Max Length 50")]
        public string CardHolderName { get; set; }

        [DisplayName("Card Number")]
        [Required(ErrorMessage = "The Card Number required.")]
        [StringLength(19, ErrorMessage = "Card Number Max Length 16")]
        [MinLength(19, ErrorMessage = "Invalid Card Number")]
        public string CardNumber { get; set; }

        [DisplayName("CVV Number")]
        [Required(ErrorMessage = "The CVV Number required.")]
        [StringLength(3, ErrorMessage = "Invalid CVV ")]
        [Range(111, 999, ErrorMessage = "Invalid CVV ")]
        public string CardCVVNo { get; set; }

        [DisplayName("Card Expiry Month")]
        // [StringLength(2, ErrorMessage = "Invalid Expiry Month")]
        [Required(ErrorMessage = "Expiry Month required.")]
        [Range(1, 12, ErrorMessage = "Invalid Expiry Month")]
        public int CardExpMonth { get; set; }

        [DisplayName("Card Expiry Year")]
        [Required(ErrorMessage = "The Card Expiry Year required.")]
        [RegularExpression(@"([0000-9999]+)", ErrorMessage = "Must be a Number.")]
        [Range(1111, 9999, ErrorMessage = "Invalid Expiry Year")]
        public int CardExpYear { get; set; }
        //-------------------------------------------------------------
        public bool? CanAccessAsAnAgent { get; set; }
        public long ID { get; set; }
        public Nullable<long> MasterID { get; set; }


        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Invalid Zipcode")]
        public string Zipcode { get; set; }

        public string LogoImage { get; set; }
        public string BrokerImage { get; set; }

    }
}