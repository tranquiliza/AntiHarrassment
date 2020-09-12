using System;

namespace AntiHarassment.Contract
{
    public class UserAuthenticatedModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
