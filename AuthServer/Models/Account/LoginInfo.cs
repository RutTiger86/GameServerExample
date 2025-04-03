using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Models.Account
{
    public class LoginInfo
    {
        public required string AccountId { get; set; }
        public string? PasswordHash { get; set; }
    }
}
