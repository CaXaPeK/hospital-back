using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.General
{
    public class TokenResponseModel
    {
        [Required]
        [MinLength(1)]
        public string Token { get; set; }
    }
}
