using System.ComponentModel.DataAnnotations;

namespace PowerIBroker.Models
{
    public class EnquiryForm
    {
        [Required(ErrorMessage = "Please provide your Name")]
        [MaxLength(500)]
        public string ContactUsName { get; set; }
        [Required(ErrorMessage = "Please provide Email.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Email is not valid")]
        [MaxLength(250)]
        public string ContactUsEmail { get; set; }
        [Required(ErrorMessage = "Please provide your Phone")]
        [MaxLength(500)]
        public string ContactUsPhone { get; set; }
        [MaxLength(2000)]
        public string Query { get; set; }
    }
}