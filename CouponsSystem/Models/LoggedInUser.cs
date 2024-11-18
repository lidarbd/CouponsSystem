using System.ComponentModel.DataAnnotations;

namespace CouponsSystem.Models
{
    public class LoggedInUser
    {
        [Key]
        public int UserId { get; set; }
    }
}
