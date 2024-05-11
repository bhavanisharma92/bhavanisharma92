using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bhaktimarg.Models
{
    public class Contactdto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        [StringLength(100, MinimumLength = 100)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email Id is Required")]
        [DataType(DataType.EmailAddress)]

        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter correct email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phonenumber is Required")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Please Enter Valid Mobile Number.")]
        public string MobileNumber { get; set; }
        public string HostingType { get; set; }
        [Required(ErrorMessage = "Message is Required")]
        [StringLength(100, MinimumLength = 3)]
        public string Message { get; set; }
        public Nullable<System.DateTime> CreatedDateTime { get; set; }
    }
}