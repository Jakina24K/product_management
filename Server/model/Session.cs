using System.ComponentModel.DataAnnotations.Schema;

namespace Server.model
{
    [Table("user_session")]
    public class Session
    {
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("duration")]
        public int Duration { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

    }
}