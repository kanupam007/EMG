using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace PowerIBroker.Areas.Broker.Models
{
    public class Company
    {
        public long ID { get; set; }


        public string CustomerCode { get; set; }
        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(150, ErrorMessage = "Company name max length 150")]
        public string CCompanyName { get; set; }

        [Required(ErrorMessage = "Street name is required.")]
        public string CStreetName { get; set; }


        [DisplayName("City")]
        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, ErrorMessage = "City max length 100")]
        public string CCityName { get; set; }

        [DisplayName("State")]
        [Required(ErrorMessage = "State is required.")]
        [StringLength(100, ErrorMessage = "State max length 100")]
        public string CStateName { get; set; }

        [DisplayName("Country")]
        [Required(ErrorMessage = "Country is required.")]
        [StringLength(100, ErrorMessage = "Country max length 100")]
        public string CCountryName { get; set; }
        [Required(ErrorMessage = "Main telephone is required.")]
        public string CContact { get; set; }


        [Required(ErrorMessage = "Zip Code is required.")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Invalid Zip Code")]
        public string CZipCode { get; set; }

        public bool IsActive { get; set; }


        public string URL { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter valid email")]
        [StringLength(50, ErrorMessage = "Email max length 50 ")]
        public string CEmail { get; set; }
        public List<CompanyClient> Clients { get; set; }
        //public CompanyBroker Broker { get; set; }
        public Billing Billing { get; set; }
        public long? BrokerId { get; set; }
        public string SubBrokerId { get; set; }
        public DateTime? BrokerStartDate { get; set; }
        public DateTime? BrokerEndDate { get; set; }
        public string ISA05IDQualifierFedTaxID { get; set; }
        public string ISA06SenderID { get; set; }
        public string ISA07IDQualifier { get; set; }
        public string ISA08ReceiverID { get; set; }



    }
    public class CompanyNew
    {
        public long ID { get; set; }
        public bool IsActive { get; set; }
        public string CustomerCode { get; set; }
        public string CCompanyName { get; set; }
        public string CStreetName { get; set; }
        public string CCountry { get; set; }
        public string CState { get; set; }
        public string CContact { get; set; }
        public string CCity { get; set; }
        public string CZipCode { get; set; }
        public string CURL { get; set; }
        public int CBrokerId { get; set; }
        public string CEmail { get; set; }
        public string BCompanyName { get; set; }
        public string Battention { get; set; }
        public string BStreetName { get; set; }
        public string BCountry { get; set; }
        public string BState { get; set; }
        public string BCity { get; set; }
        public string BZipCode { get; set; }
        public string BPhone { get; set; }
        public string BFax { get; set; }
        public bool BIsActive { get; set; }
        public bool BIsDeleted { get; set; }
        public long BId { get; set; }
        public List<CompanyClientNew> ClientDetails { get; set; }
        public List<long> SubBrokerIds { get; set; }
        //public List<RentalFlat> FlatDetails { get; set; }
    }
    
    public class CompanyClientNew
    {
        public long ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int AdministratorType { get; set; }
        public string Phone { get; set; }
        public string WorkPhone { get; set; }
        public bool IsActive { get; set; }
    }
    public class CompanyClient
    {
        public long ID { get; set; }
        [DisplayName("First Name")]
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required(ErrorMessage = " Last Name is required.")]
        public string LastName { get; set; }


        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter valid email")]
        [StringLength(50, ErrorMessage = "Email Max Length 50 ")]
        public string Email { get; set; }

        public int? AdministratorType { get; set; }

        //[Required(ErrorMessage = "Mobile phone is required.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Work phone is required.")]
        public string WorkPhone { get; set; }

        public bool IsActive { get; set; }
    }



    public class Broker
    {
        public long SubBrokerId { get; set; }
        public long? BrokerId { get; set; }
        [Required(ErrorMessage = "Broker company name is required.")]
        public long? ddlBrokerId { get; set; }
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Broker company name is required.")]
        public string BrokerName { get; set; }
        public string LastName { get; set; }
        [Required(ErrorMessage = "Street name is required.")]
        public string Street { get; set; }
        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, ErrorMessage = "City max length 100")]
        public string City { get; set; }
        [Required(ErrorMessage = "State is required.")]
        [StringLength(100, ErrorMessage = "State max length 100")]
        public string State { get; set; }
        [Required(ErrorMessage = "Zip Code is required.")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Invalid broker Zip Code")]
        public string ZipCode { get; set; }
        [Required(ErrorMessage = "Country is required.")]
        [StringLength(100, ErrorMessage = "Country max length 100")]
        public string Country { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter valid email")]
        [StringLength(50, ErrorMessage = "Email max length 50 ")]
        public string Email { get; set; }
        public string WorkPhone { get; set; }
        //[Required(ErrorMessage = "Mobile phone is required.")]
        public string Phone { get; set; }
    }
    public class BrokerNew
    {
        public long SubBrokerId { get; set; }
        public long BrokerId { get; set; }
        public string BrokerName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string WorkPhone { get; set; }
        public string Phone { get; set; }
        public int AdministratorType { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string BrokerPhone { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        
        
    }
    public class Billing
    {
        public long BillingId { get; set; }
        [Required(ErrorMessage = "Company name is required.")]
        public string CompanyName { get; set; }
        public long CompanyId { get; set; }
        [Required(ErrorMessage = "Street name is required.")]
        public string StreetName { get; set; }
        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }
        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }
        [Required(ErrorMessage = " Zip Code required.")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Invalid Zip Code")]
        public string ZipCode { get; set; }
        [Required(ErrorMessage = "Country  is required.")]
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
    }

    public class CompanyBroker
    {
        public long BrokerId { get; set; }
        [Required(ErrorMessage = "Broker company name is required.")]
        public string BrokerName { get; set; }

        [Required(ErrorMessage = "Street name is required.")]
        public string Street { get; set; }
        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, ErrorMessage = "City max length 100")]
        public string City { get; set; }
        [Required(ErrorMessage = "State is required.")]
        [StringLength(100, ErrorMessage = "State max length 100")]
        public string State { get; set; }
        [Required(ErrorMessage = "Zip Code is required.")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Invalid broker Zip Code")]
        public string ZipCode { get; set; }
        [Required(ErrorMessage = "Country is required.")]
        [StringLength(100, ErrorMessage = "Country max length 100")]
        public string Country { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter valid email")]
        [StringLength(50, ErrorMessage = "Email max length 50 ")]
        public string Email { get; set; }
        public string WorkPhone { get; set; }
        [Required(ErrorMessage = "Mobile phone is required.")]
        public string Phone { get; set; }
        public List<Broker> Clients { get; set; }
    }


}