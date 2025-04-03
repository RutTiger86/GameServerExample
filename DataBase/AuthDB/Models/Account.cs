using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthDB.Models
{
    public class Account
    {
        public long Id { get; set; }
        public LoginType LoginType { get; set; } = LoginType.ID_PASSWORD;
        public required string AccountId { get; set; }
        public required string AccountName { get; set; }
        public string? PasswordHash { get; set; }
        public required DateTime CreateTime { get; set; }
        public ICollection<Character> Characters { get; set; } = new List<Character>();
    }
}
