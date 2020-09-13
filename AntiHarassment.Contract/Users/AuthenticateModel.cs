using System.ComponentModel.DataAnnotations;

namespace AntiHarassment.Contract
{
    public class AuthenticateModel
    {
        [Required]
        public string TwitchUsername { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
