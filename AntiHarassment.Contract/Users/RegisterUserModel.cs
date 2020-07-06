using System;
using System.Collections.Generic;
using System.Text;

namespace AntiHarassment.Contract
{
    public class RegisterUserModel
    {
        public string TwitchUsername { get; set; }
        
        public string Email { get; set; }

        public string Password { get; set; }
    }
}