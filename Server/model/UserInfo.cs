using System.ComponentModel.DataAnnotations.Schema;

namespace Server.model {
    public class UserInfo {
        [Column("id")]
        public int? Id { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("password")]
        public string? Password { get; set; }

    }
}