using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffeMenuBot.Data.Models.Authentication
{
    [Table("app_users", Schema = "public")]
    public sealed record ApplicationUser
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required, Column("user_id", TypeName = "integer")]
        public int Id { get; init; }

        [Column("username", TypeName = "text"), Required]
        public string Username { get; init; } = null!;

        [Column("password_hash", TypeName = "text"), Required]
        public string PasswordHash { get; init; } = null!;

        [Column("password_salt", TypeName = "text"), Required]
        public string Salt { get; init; } = null!;

        [Column("user_role", TypeName = "text"), Required]
        public string Role { get; init; } = null!;

    }
}
