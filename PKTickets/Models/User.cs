using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PKTickets.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Please Enter the User Name !!!")]
        [StringLength(20, ErrorMessage = "User Name length must be below 20 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please Enter the Phone Number !!!")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "PhoneNumber must be 10 Digits")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please Enter the EmailId !!!")]
        [RegularExpression("^[a-z0-9](\\.?[a-z0-9]){5,}@g(oogle)?mail\\.com$")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(20, ErrorMessage = "Password must be between 5 and 20 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please Enter the Location !!!")]
        [StringLength(20, ErrorMessage = "Location length must be below 20 characters")]
        public string Location { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
