using System.ComponentModel.DataAnnotations;

namespace AuthDB.Models
{
    public class Account
    {
        public long Id { get; set; }
        public LoginType LoginType { get; set; } = LoginType.ID_PASSWORD;

        [MaxLength(320)] // 이메일 최대 길이
        public required string AccountId { get; set; }

        [MaxLength(50)] // 사용자 이름 제한
        public required string AccountName { get; set; }

        [MaxLength(32)]
        public byte[]? PasswordHash { get; set; }

        [MaxLength(16)]
        public byte[]? Salt { get; set; }

        public required DateTime CreateTime { get; set; }
        public ICollection<Character> Characters { get; set; } = new List<Character>();
    }
}
