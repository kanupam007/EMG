using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PowerIBroker.Models
{
    public class AdminLogin
    {
        [Required(ErrorMessage = "Please provide your First Name")]
        [MaxLength(500)]
        public string EmailOrUserName { get; set; }
        [Required(ErrorMessage = "Please provide your Password")]
        [MaxLength(500)]
        public string Password { get; set; }
        public bool? RememberMe { get; set; }


        public bool Success { get; set; }
        public List<string> ErrorMessage { get; set; }
        public string CultureName { get; set; }
    }
}